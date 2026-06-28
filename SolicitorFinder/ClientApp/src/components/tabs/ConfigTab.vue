<template>
    <div class="config-tab">

        <div v-if="loading" class="loader-overlay">
            <div class="loader">
                <div class="spinner"></div>
                <p>Loading configuration...</p>
            </div>
        </div>

        <h2>⚙️ Configuration</h2>
        <p class="subtitle">Settings and favorite locations</p>

        <div class="config-form">
            <div class="form-group">
                <label for="updateInterval">🔄 Interval in minutes</label>
                <input id="updateInterval"
                       type="number"
                       v-model.number="config.updateInterval"
                       min="1"
                       max="1440" />
                <small>How often to update data from external sources</small>
            </div>

            <div class="form-group checkbox-group">
                <label class="checkbox-label">
                    <input type="checkbox" v-model="config.autoUpdate" />
                    <span>Automatic Updates</span>
                </label>
                <small>Enable automatic data updates</small>
            </div>

            <div class="form-group">
                <label for="maxResults">📊 Max Results per page</label>
                <input id="maxResults"
                       type="number"
                       v-model.number="config.maxResults"
                       min="5"
                       max="100" />
                <small>Maximum number of results to display per page</small>
            </div>

            <div class="locations-section">
                <h3>📍 Favorite Locations</h3>
                <p class="subtitle-small">Search and add locations to favorites</p>

                <div class="location-search">
                    <div class="search-row">
                        <input type="text"
                               v-model="locationSearchQuery"
                               placeholder="Type at least 2 characters..."
                               @input="onSearchInput"
                               @keyup.enter="searchLocations" />
                        <button class="btn btn-search" @click="searchLocations" :disabled="loadingSearch || locationSearchQuery.length < 2">
                            {{ loadingSearch ? 'Searching...' : '🔍 Search' }}
                        </button>
                    </div>
                </div>

                <div v-if="searchResults.length > 0" class="search-results">
                    <div class="result-item"
                         v-for="location in searchResults"
                         :key="location.title + location.text">
                        <span class="location-name">{{ location.title }} - {{ location.text }}</span>
                        <button class="btn-add"
                                @click="addLocationToFavorites(location)"
                                :disabled="isLocationInConfig(location)">
                            {{ isLocationInConfig(location) ? '✅ Added' : '➕ Add' }}
                        </button>
                    </div>
                </div>

                <div v-if="searchResults.length === 0 && locationSearchQuery.length >= 2 && !loadingSearch" class="no-results">
                    No locations found for "{{ locationSearchQuery }}"
                </div>

                <div class="favorite-locations">
                    <h4>⭐ Favorite Locations ({{ favoriteLocations.length }})</h4>

                    <div v-if="favoriteLocations.length > 0" class="favorite-list">
                        <div class="favorite-item"
                             v-for="(location, index) in favoriteLocations"
                             :key="index">
                            <span class="location-name">{{ location.title }} - {{ location.text }}</span>
                            <button class="btn-remove" @click="removeLocationFromFavorites(index)" title="Remove from favorites">
                                ✕
                            </button>
                        </div>
                    </div>
                    <div v-else class="no-favorites">
                        No favorite locations added yet
                    </div>
                </div>
            </div>

            <!-- Save Button -->
            <div class="form-actions">
                <button class="btn btn-primary" @click="saveConfig" :disabled="saving">
                    {{ saving ? 'Saving...' : '💾 Save configuration' }}
                </button>
            </div>

            <div v-if="saveMessage" class="save-message" :class="saveMessageType">
                {{ saveMessage }}
            </div>
        </div>
    </div>
</template>

<script>
    import { ref, reactive, onMounted } from 'vue'
    import { solicitorApi } from '../../api/solicitorApi.js'
    import { locationApi } from '../../api/locationApi.js'
    import { Config } from '../../models/config.js'

    export default {
        name: 'ConfigTab',
        setup() {
            const config = reactive(new Config())
            const saving = ref(false)
            const saveMessage = ref('')
            const saveMessageType = ref('')

            const locationSearchQuery = ref('')
            const searchResults = ref([])
            const loadingSearch = ref(false)
            let searchTimeout = null
            const loading = ref(true)

            const favoriteLocations = ref([])

            const loadConfig = async () => {
                loading.value = true

                try {
                    const data = await solicitorApi.getConfig()
                    Object.assign(config, new Config(data))
                    favoriteLocations.value = config.locations || []
                } catch (err) {
                    console.error('Failed to load config:', err)
                } finally {
                    loading.value = false
                }
            }

            const searchLocations = async () => {
                const query = locationSearchQuery.value.trim()
                if (query.length < 2) {
                    searchResults.value = []
                    return
                }

                loadingSearch.value = true
                searchResults.value = []

                try {
                    const results = await locationApi.searchLocations(query)
                    searchResults.value = results
                } catch (error) {
                    console.error('Search error:', error)
                    searchResults.value = []
                } finally {
                    loadingSearch.value = false
                }
            }

            const onSearchInput = () => {
                if (searchTimeout) {
                    clearTimeout(searchTimeout)
                }

                searchTimeout = setTimeout(() => {
                    if (locationSearchQuery.value.trim().length >= 2) {
                        searchLocations()
                    } else {
                        searchResults.value = []
                    }
                }, 500)
            }

            const saveConfig = async () => {
                saving.value = true
                saveMessage.value = ''
                try {
                    const payload = {
                        updateInterval: config.updateInterval,
                        autoUpdate: config.autoUpdate,
                        maxResults: config.maxResults,
                        locations: favoriteLocations.value.map(l => ({
                            title: l.title,
                            text: l.text
                        }))
                    }

                    await solicitorApi.updateConfig(payload)
                    saveMessage.value = '✅ Configuration saved successfully!'
                    saveMessageType.value = 'success'
                    setTimeout(() => { saveMessage.value = '' }, 3000)
                } catch (err) {
                    saveMessage.value = '❌ Error: ' + (err.response?.data?.message || err.message)
                    saveMessageType.value = 'error'
                } finally {
                    saving.value = false
                }
            }

            const addLocationToFavorites = (location) => {
                if (favoriteLocations.value.some(l => l.title === location.title && l.text === location.text)) {
                    return
                }
                favoriteLocations.value.push({
                    title: location.title,
                    text: location.text
                })
            }

            const removeLocationFromFavorites = (index) => {
                favoriteLocations.value.splice(index, 1)
            }

            const isLocationInConfig = (location) => {
                return favoriteLocations.value.some(
                    l => l.title === location.title && l.text === location.text
                )
            }

            onMounted(() => {
                loadConfig()
            })

            return {
                config,
                saving,
                saveMessage,
                saveMessageType,
                locationSearchQuery,
                searchResults,
                loadingSearch,
                favoriteLocations,
                searchLocations,
                onSearchInput,
                saveConfig,
                addLocationToFavorites,
                removeLocationFromFavorites,
                isLocationInConfig,
                loading,
            }
        }
    }
</script>

<style scoped>
    .config-tab {
        max-width: 800px;
        position: relative;
        min-height: 400px;
    }

        .config-tab h2 {
            margin-bottom: 4px;
            color: #2d3748;
        }

    .subtitle {
        color: #718096;
        margin-bottom: 30px;
    }

    .config-form {
        background: white;
        padding: 30px;
        border-radius: 12px;
        box-shadow: 0 2px 8px rgba(0, 0, 0, 0.06);
    }

    .loader-overlay {
        position: absolute;
        top: 0;
        left: 0;
        right: 0;
        bottom: 0;
        background: rgba(255, 255, 255, 0.85);
        display: flex;
        align-items: center;
        justify-content: center;
        border-radius: 12px;
        z-index: 10;
        min-height: 300px;
    }

    .loader {
        text-align: center;
    }

        .loader p {
            color: #718096;
            font-size: 14px;
            margin-top: 12px;
        }

    .spinner {
        display: inline-block;
        width: 40px;
        height: 40px;
        border: 4px solid #e2e8f0;
        border-top-color: #2d3748;
        border-radius: 50%;
        animation: spin 0.8s linear infinite;
    }

    @keyframes spin {
        to {
            transform: rotate(360deg);
        }
    }

    .form-group {
        margin-bottom: 20px;
    }

        .form-group label {
            display: block;
            font-weight: 600;
            color: #2d3748;
            margin-bottom: 6px;
        }

        .form-group input[type="number"],
        .form-group input[type="text"],
        .form-group input[type="email"] {
            width: 100%;
            padding: 10px 14px;
            border: 2px solid #e2e8f0;
            border-radius: 8px;
            font-size: 16px;
            transition: all 0.2s;
        }

        .form-group input:focus {
            outline: none;
            border-color: #4a5568;
            box-shadow: 0 0 0 3px rgba(74, 85, 104, 0.1);
        }

        .form-group small {
            display: block;
            margin-top: 4px;
            color: #a0aec0;
            font-size: 13px;
        }

    .checkbox-group {
        padding: 12px 0;
    }

    .checkbox-label {
        display: flex !important;
        align-items: center;
        gap: 10px;
        cursor: pointer;
        font-weight: 500 !important;
    }

        .checkbox-label input[type="checkbox"] {
            width: 20px;
            height: 20px;
            cursor: pointer;
        }

    .locations-section {
        margin-top: 30px;
        padding-top: 30px;
        border-top: 2px solid #edf2f7;
    }

        .locations-section h3 {
            color: #2d3748;
            margin-bottom: 4px;
        }

    .subtitle-small {
        color: #718096;
        font-size: 14px;
        margin-bottom: 16px;
    }

    .location-search .search-row {
        display: flex;
        gap: 12px;
    }

    .location-search input {
        flex: 1;
        padding: 10px 14px;
        border: 2px solid #e2e8f0;
        border-radius: 8px;
        font-size: 15px;
        transition: all 0.2s;
    }

        .location-search input:focus {
            outline: none;
            border-color: #4a5568;
            box-shadow: 0 0 0 3px rgba(74, 85, 104, 0.1);
        }

    .btn {
        padding: 10px 24px;
        border: none;
        border-radius: 8px;
        font-size: 15px;
        font-weight: 600;
        cursor: pointer;
        transition: all 0.2s;
        white-space: nowrap;
    }

        .btn:disabled {
            opacity: 0.6;
            cursor: not-allowed;
        }

    .btn-search {
        background: #2d3748;
        color: white;
    }

        .btn-search:hover:not(:disabled) {
            background: #4a5568;
        }

    .btn-add {
        padding: 4px 14px;
        border: none;
        border-radius: 6px;
        background: #48bb78;
        color: white;
        font-size: 13px;
        cursor: pointer;
        transition: all 0.2s;
    }

        .btn-add:hover:not(:disabled) {
            background: #38a169;
        }

        .btn-add:disabled {
            background: #a0aec0;
            cursor: not-allowed;
        }

    .btn-remove {
        padding: 2px 8px;
        border: none;
        border-radius: 4px;
        background: #fc8181;
        color: white;
        font-size: 14px;
        cursor: pointer;
        transition: all 0.2s;
    }

        .btn-remove:hover {
            background: #e53e3e;
        }

    .search-results {
        margin-top: 12px;
        max-height: 200px;
        overflow-y: auto;
        border: 1px solid #edf2f7;
        border-radius: 8px;
    }

    .result-item {
        display: flex;
        justify-content: space-between;
        align-items: center;
        padding: 10px 14px;
        border-bottom: 1px solid #f7fafc;
    }

        .result-item:last-child {
            border-bottom: none;
        }

        .result-item:hover {
            background: #f7fafc;
        }

    .location-name {
        font-size: 14px;
        color: #2d3748;
    }

    .no-results {
        padding: 16px;
        text-align: center;
        color: #a0aec0;
        font-size: 14px;
    }

    .favorite-locations {
        margin-top: 20px;
    }

        .favorite-locations h4 {
            color: #2d3748;
            margin-bottom: 12px;
        }

    .favorite-list {
        border: 1px solid #edf2f7;
        border-radius: 8px;
    }

    .favorite-item {
        display: flex;
        justify-content: space-between;
        align-items: center;
        padding: 10px 14px;
        border-bottom: 1px solid #f7fafc;
    }

        .favorite-item:last-child {
            border-bottom: none;
        }

        .favorite-item:hover {
            background: #f7fafc;
        }

    .no-favorites {
        padding: 20px;
        text-align: center;
        color: #a0aec0;
        font-size: 14px;
        background: #f7fafc;
        border-radius: 8px;
    }

    .form-actions {
        display: flex;
        gap: 12px;
        margin-top: 24px;
    }

    .btn-primary {
        background: #2d3748;
        color: white;
    }

        .btn-primary:hover:not(:disabled) {
            background: #4a5568;
            transform: translateY(-1px);
        }

    .save-message {
        margin-top: 16px;
        padding: 12px 16px;
        border-radius: 8px;
        font-weight: 500;
    }

        .save-message.success {
            background: #c6f6d5;
            color: #22543d;
        }

        .save-message.error {
            background: #fed7d7;
            color: #742a2a;
        }

        .save-message.info {
            background: #bee3f8;
            color: #2a4365;
        }

    @media (max-width: 768px) {
        .location-search .search-row {
            flex-direction: column;
        }

        .btn {
            width: 100%;
            justify-content: center;
        }

        .result-item {
            flex-direction: column;
            gap: 8px;
            align-items: stretch;
        }

        .config-form {
            padding: 16px;
        }
    }
</style>