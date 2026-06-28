<template>
    <div class="list-tab">
        <div class="list-header">
            <h2>📋 Solicitor list</h2>
            <span class="count" v-if="pagedResult?.totalCount > 0">
                {{ pagedResult.totalCount }}
            </span>
        </div>

        <div class="filters">
            <div class="filter-row">
                <div class="filter-group">
                    <label>📍 Location <span class="required">*</span></label>
                    <select v-model="filter.locationId">
                        <option value="">Select location</option>
                        <option v-for="loc in (config?.locations || [])" :key="loc.id" :value="loc.id">
                            {{ loc.name }}
                        </option>
                    </select>
                </div>
                <div class="filter-group">
                    <label>🏢 Area <span class="required">*</span></label>
                    <select v-model="filter.areaId">
                        <option value="">Select area</option>
                        <option v-for="area in (config?.areas || [])" :key="area.id" :value="area.id">
                            {{ area.name }}
                        </option>
                    </select>
                </div>
                <div class="filter-group">
                    <label>🔍 Search</label>
                    <input type="text"
                           v-model="filter.searchTerm"
                           placeholder="Search..."
                           @keyup.enter="applyFilters" />
                </div>
            </div>

            <div class="filter-row">
                <div class="filter-group">
                    <label>⭐ Ratings</label>
                    <div class="rating-filter">
                        <input type="number"
                               v-model.number="filter.minRating"
                               placeholder="from"
                               min="0"
                               max="5"
                               step="0.5" />
                        <span>—</span>
                        <input type="number"
                               v-model.number="filter.maxRating"
                               placeholder="to"
                               min="0"
                               max="5"
                               step="0.5" />
                    </div>
                </div>
                <div class="filter-group">
                    <label>💬 Reviews</label>
                    <div class="rating-filter">
                        <input type="number"
                               v-model.number="filter.minReviewCount"
                               placeholder="from"
                               min="0" />
                        <span>—</span>
                        <input type="number"
                               v-model.number="filter.maxReviewCount"
                               placeholder="to"
                               min="0" />
                    </div>
                </div>
                <div class="filter-group">
                    <label>📄 Per page</label>
                    <span class="page-size-display">{{ filter.pageSize }}</span>
                    <small class="hint">From config</small>
                </div>
            </div>

            <div class="filter-row">
                <div class="filter-group">
                    <label>Sort by</label>
                    <div class="sort-group">
                        <select v-model="filter.sortBy" @change="applyFilters">
                            <option value="RatingStars">⭐ Rating</option>
                            <option value="Name">📝 Name</option>
                            <option value="ReviewCount">💬 Reviews</option>
                            <option value="CreatedAt">📅 Date</option>
                        </select>
                        <button class="sort-direction"
                                :class="{ active: true, desc: filter.sortDescending, asc: !filter.sortDescending }"
                                @click="toggleSortDirection"
                                :title="filter.sortDescending ? 'Descending' : 'Ascending'">
                            {{ filter.sortDescending ? '↓' : '↑' }}
                        </button>
                    </div>
                </div>
                <div class="filter-group filter-actions-group">
                    <button class="btn btn-primary" @click="applyFilters" :disabled="loading || !canSearch">
                        {{ loading ? '⏳ Loading...' : '🔍 Search' }}
                    </button>
                    <button class="btn btn-secondary" @click="resetAllFilters">✕ Clear</button>
                </div>
            </div>

        </div>

        <div v-if="loading" class="loading-state">
            <div class="spinner"></div>
            <p>Loading...</p>
        </div>

        <div v-else-if="error" class="error-state">
            <p>❌ {{ error }}</p>
        </div>

        <div v-else-if="solicitors.length === 0" class="empty-state">
            <p>🔎 No info</p>
            <p class="hint">Try to change search options</p>
        </div>

        <div v-else class="results">
            <div class="results-grid">
                <SolicitorCard v-for="solicitor in solicitors"
                               :key="solicitor.id"
                               :solicitor="solicitor" />
            </div>

            <Pagination :current-page="pagedResult.page"
                        :total-pages="pagedResult.totalPages"
                        :total-count="pagedResult.totalCount"
                        @page-change="onPageChange" />
        </div>
    </div>
</template>

<script>
    import { onMounted, computed, watch } from 'vue'
    import { useSolicitors } from '../../composables/useSolicitors.js'
    import SolicitorCard from '../common/SolicitorCard.vue'
    import Pagination from '../common/Pagination.vue'

    export default {
        name: 'ListTab',
        components: {
            SolicitorCard,
            Pagination
        },
        setup() {
            const {
                solicitors,
                pagedResult,
                loading,
                error,
                filter,
                config,
                searchSolicitors,
                resetFilters,
                changePage,
                changePageSize,
                init
            } = useSolicitors()

            if (!config.locations) config.locations = []
            if (!config.areas) config.areas = []

            const canSearch = computed(() => {
                return !!(filter.locationId || filter.areaId)
            })

            const applyFilters = () => {
                if (!canSearch.value) {
                    return
                }
                filter.page = 1
                searchSolicitors()
            }

            const resetAllFilters = () => {
                resetFilters()
                setDefaultLocation()
                if (canSearch.value) {
                    searchSolicitors()
                }
            }

            const setDefaultLocation = () => {
                if (config.locations && config.locations.length > 0) {
                    filter.locationId = config.locations[0].id
                }
                if (config.defaultLocation) {
                    const found = config.locations.find(l =>
                        l.name?.toLowerCase() === config.defaultLocation?.toLowerCase()
                    )
                    if (found) {
                        filter.locationId = found.id
                    }
                }
            }

            const onFilterChange = () => {
            }

            const onPageChange = (page) => {
                changePage(page)
            }

            const toggleSortDirection = () => {
                filter.sortDescending = !filter.sortDescending
                applyFilters()
            }

            onMounted(async () => {
                await init()
                setDefaultLocation()
                if (canSearch.value) {
                    await searchSolicitors()
                }
            })

            return {
                solicitors,
                pagedResult,
                loading,
                error,
                filter,
                config,
                canSearch,
                applyFilters,
                resetAllFilters,
                onPageChange,
                toggleSortDirection
            }
        }
    }
</script>

<style scoped>
    .list-tab {
        width: 100%;
    }

    .list-header {
        display: flex;
        justify-content: space-between;
        align-items: center;
        margin-bottom: 24px;
    }

        .list-header h2 {
            color: #2d3748;
            margin: 0;
        }

    .count {
        color: #718096;
        font-size: 14px;
        background: #edf2f7;
        padding: 4px 14px;
        border-radius: 20px;
    }

    .filters {
        background: white;
        padding: 24px;
        border-radius: 12px;
        box-shadow: 0 2px 8px rgba(0, 0, 0, 0.06);
        margin-bottom: 24px;
    }

    .filter-row {
        display: flex;
        gap: 16px;
        margin-bottom: 12px;
        flex-wrap: wrap;
        align-items: flex-end;
    }

    .filter-group {
        flex: 1;
        min-width: 140px;
    }

        .filter-group label {
            display: block;
            font-size: 13px;
            font-weight: 600;
            color: #4a5568;
            margin-bottom: 4px;
        }

    .required {
        color: #e53e3e;
    }

    .filter-group input,
    .filter-group select {
        width: 100%;
        padding: 8px 12px;
        border: 2px solid #e2e8f0;
        border-radius: 8px;
        font-size: 14px;
        transition: all 0.2s;
    }

        .filter-group input:focus,
        .filter-group select:focus {
            outline: none;
            border-color: #4a5568;
            box-shadow: 0 0 0 3px rgba(74, 85, 104, 0.1);
        }

    .page-size-display {
        display: inline-block;
        padding: 8px 16px;
        background: #f7fafc;
        border-radius: 8px;
        font-weight: 600;
        color: #2d3748;
        font-size: 15px;
        min-width: 60px;
        text-align: center;
    }

    .hint {
        display: block;
        font-size: 12px;
        color: #a0aec0;
        margin-top: 2px;
    }

    .rating-filter {
        display: flex;
        align-items: center;
        gap: 8px;
    }

        .rating-filter input {
            width: 120px;
            text-align: center;
        }

            .rating-filter input::-webkit-inner-spin-button {
                opacity: 0.5;
            }

    .sort-group {
        display: flex;
        gap: 8px;
    }

        .sort-group select {
            flex: 1;
        }

    .sort-direction {
        padding: 8px 12px;
        border: 2px solid #e2e8f0;
        border-radius: 8px;
        background: white;
        cursor: pointer;
        font-size: 16px;
        transition: all 0.2s;
        min-width: 44px;
        font-weight: 700;
    }

        .sort-direction:hover {
            background: #edf2f7;
            border-color: #4a5568;
        }

        .sort-direction.desc {
            background: #2d3748;
            color: white;
            border-color: #2d3748;
        }

            .sort-direction.desc:hover {
                background: #4a5568;
            }

        .sort-direction.asc {
            background: #edf2f7;
            color: #2d3748;
            border-color: #cbd5e0;
        }

            .sort-direction.asc:hover {
                background: #e2e8f0;
            }

    .filter-actions-group {
        display: flex;
        gap: 12px;
        align-items: flex-end;
    }

        .filter-actions-group .btn {
            flex: 1;
        }

    .filter-error {
        margin-top: 12px;
        padding: 10px 14px;
        background: #fed7d7;
        color: #c53030;
        border-radius: 8px;
        font-size: 14px;
    }

    .btn {
        padding: 8px 24px;
        border: none;
        border-radius: 8px;
        font-size: 14px;
        font-weight: 600;
        cursor: pointer;
        transition: all 0.2s;
    }

        .btn:disabled {
            opacity: 0.6;
            cursor: not-allowed;
        }

    .btn-primary {
        background: #2d3748;
        color: white;
    }

        .btn-primary:hover:not(:disabled) {
            background: #4a5568;
        }

    .btn-secondary {
        background: #edf2f7;
        color: #2d3748;
    }

        .btn-secondary:hover:not(:disabled) {
            background: #e2e8f0;
        }

    .results-grid {
        display: grid;
        grid-template-columns: repeat(auto-fill, minmax(360px, 1fr));
        gap: 20px;
    }

    .loading-state,
    .error-state,
    .empty-state {
        text-align: center;
        padding: 60px 20px;
        background: white;
        border-radius: 12px;
        box-shadow: 0 2px 8px rgba(0, 0, 0, 0.06);
    }

        .loading-state .spinner {
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

    .error-state p {
        color: #e53e3e;
        font-size: 16px;
    }

    .empty-state p {
        color: #718096;
        font-size: 18px;
    }

    .empty-state .hint {
        font-size: 14px;
        color: #a0aec0;
        margin-top: 4px;
    }

    @media (max-width: 768px) {
        .filter-row {
            flex-direction: column;
        }

        .filter-group {
            min-width: 100%;
        }

        .results-grid {
            grid-template-columns: 1fr;
        }

        .filter-actions-group {
            flex-direction: column;
        }

            .filter-actions-group .btn {
                width: 100%;
            }
    }
</style>