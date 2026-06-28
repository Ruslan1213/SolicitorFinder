<template>
    <div class="reports-tab">
        <div class="reports-header">
            <div>
                <h2>📊 Reposrts and statistics</h2>
                <p class="subtitle">analytics by solicitors</p>
            </div>
            <button class="btn btn-primary" @click="refreshReports" :disabled="loading">
                {{ loading ? '⏳ Updating...' : '🔄 Update' }}
            </button>
        </div>

        <div class="stats-grid">
            <div class="stat-card">
                <div class="stat-icon">👥</div>
                <div class="stat-content">
                    <div class="stat-value">{{ stats.totalSolicitors }}</div>
                    <div class="stat-label">Total Solicitors</div>
                </div>
            </div>
            <div class="stat-card">
                <div class="stat-icon">⭐</div>
                <div class="stat-content">
                    <div class="stat-value">{{ stats.averageRating }}</div>
                    <div class="stat-label">Average Rating</div>
                </div>
            </div>
            <div class="stat-card">
                <div class="stat-icon">📍</div>
                <div class="stat-content">
                    <div class="stat-value">{{ stats.totalLocations }}</div>
                    <div class="stat-label">Locations</div>
                </div>
            </div>
            <div class="stat-card">
                <div class="stat-icon">📅</div>
                <div class="stat-content">
                    <div class="stat-value">{{ stats.lastUpdated }}</div>
                    <div class="stat-label">Last Updated</div>
                </div>
            </div>
        </div>

        <div class="reports-grid">
            <div class="report-card">
                <h3>🏆 Best of the best</h3>
                <div v-if="loading" class="loading-small">
                    <div class="spinner-small"></div>
                </div>
                <div v-else-if="topRated.length === 0" class="empty-small">
                    No data
                </div>
                <div v-else class="report-list">
                    <div v-for="(item, index) in topRated"
                         :key="item.id"
                         class="report-item"
                         :class="{ 'gold': index === 0, 'silver': index === 1, 'bronze': index === 2 }">
                        <span class="rank">{{ index + 1 }}</span>
                        <span class="name">{{ item.name || 'Без имени' }}</span>
                        <span class="rating">⭐ {{ item.ratingStars }}</span>
                        <span class="location">{{ item.location || '—' }}</span>
                    </div>
                </div>
            </div>

            <div class="report-card">
                <h3>💬 Top 5 by reviews</h3>
                <div v-if="loading" class="loading-small">
                    <div class="spinner-small"></div>
                </div>
                <div v-else-if="topReviewed.length === 0" class="empty-small">
                    No data
                </div>
                <div v-else class="report-list">
                    <div v-for="(item, index) in topReviewed"
                         :key="item.id"
                         class="report-item">
                        <span class="rank">{{ index + 1 }}</span>
                        <span class="name">{{ item.name || 'Без имени' }}</span>
                        <span class="rating">💬 {{ item.reviewCount }}</span>
                        <span class="location">{{ item.location || '—' }}</span>
                    </div>
                </div>
            </div>
        </div>

        <div class="report-card full-width">
            <h3>
                📊 Distribution by rating
            </h3>
            <div v-if="loading" class="loading-small">
                <div class="spinner-small"></div>
            </div>
            <div v-else-if="ratingDistribution.length === 0" class="empty-small">
                No data
            </div>
            <div v-else class="distribution">
                <div v-for="item in ratingDistribution"
                     :key="item.range"
                     class="distribution-bar">
                    <span class="range-label">{{ item.range }}</span>
                    <div class="bar-track">
                        <div class="bar-fill"
                             :style="{ width: item.percentage + '%' }"
                             :class="getBarColor(item.range)"></div>
                    </div>
                    <span class="count-label">{{ item.count }} ({{ item.percentage }}%)</span>
                </div>
            </div>
        </div>

        <div class="report-card full-width" v-if="locationStats.length > 0 || loading">
            <h3>📍 Stats by city</h3>
            <div v-if="loading" class="loading-small">
                <div class="spinner-small"></div>
            </div>
            <div v-else class="location-table">
                <div class="location-row location-header">
                    <span>City</span>
                    <span>Solicitors</span>
                    <span>Avg. Rating</span>
                    <span>Reviews</span>
                </div>
                <div v-for="item in locationStats" :key="item.location" class="location-row">
                    <span class="loc-name">{{ item.location }}</span>
                    <span class="loc-count">{{ item.solicitorsCount }}</span>
                    <span class="loc-rating">⭐ {{ item.averageRating }}</span>
                    <span class="loc-reviews">💬 {{ item.totalReviews }}</span>
                </div>
            </div>
        </div>

        <div v-if="!loading && stats.totalSolicitors === 0" class="empty-state">
            <div class="empty-icon">📭</div>
            <h3>No data to displpay</h3>
            <p>load solicitors</p>
        </div>
    </div>
</template>
<script>
    import { ref, reactive, onMounted } from 'vue'
    import { solicitorApi } from '../../api/solicitorApi.js'

    export default {
        name: 'ReportsTab',
        setup() {
            const loading = ref(false)
            const stats = reactive({
                totalSolicitors: 0,
                averageRating: '0.0',
                totalLocations: 0,
                totalAreas: 0,
                totalReviews: 0,
                lastUpdated: '—'
            })
            const topRated = ref([])
            const topReviewed = ref([])
            const ratingDistribution = ref([])
            const locationStats = ref([])

            const loadReports = async () => {
                loading.value = true
                try {
                    const data = await solicitorApi.getReports(5)

                    if (data.stats) {
                        stats.totalSolicitors = data.stats.totalSolicitors || 0
                        stats.averageRating = data.stats.averageRating?.toFixed(1) || '0.0'
                        stats.totalLocations = data.stats.totalLocations || 0
                        stats.totalAreas = data.stats.totalAreas || 0
                        stats.totalReviews = data.stats.totalReviews || 0

                        if (data.stats.lastUpdated) {
                            const d = new Date(data.stats.lastUpdated)
                            stats.lastUpdated = d.toLocaleDateString('ru-RU', {
                                day: '2-digit',
                                month: '2-digit',
                                year: 'numeric',
                                hour: '2-digit',
                                minute: '2-digit'
                            })
                        }
                    }

                    topRated.value = data.topRated || []
                    topReviewed.value = data.topReviewed || []
                    ratingDistribution.value = data.ratingDistribution || []
                    locationStats.value = data.locationStats || []

                } catch (err) {
                    console.error('Failed to load reports:', err)
                } finally {
                    loading.value = false
                }
            }

            const refreshReports = () => {
                loadReports()
            }

            const getBarColor = (range) => {
                const colors = {
                    '0-1': 'bar-red',
                    '1-2': 'bar-orange',
                    '2-3': 'bar-yellow',
                    '3-4': 'bar-green',
                    '4-5': 'bar-blue'
                }
                return colors[range] || 'bar-blue'
            }

            onMounted(() => {
                loadReports()
            })

            return {
                loading,
                stats,
                topRated,
                topReviewed,
                ratingDistribution,
                locationStats,
                refreshReports,
                getBarColor
            }
        }
    }
</script>

<style scoped>
    .reports-tab {
        width: 100%;
    }

    .reports-header {
        display: flex;
        justify-content: space-between;
        align-items: center;
        margin-bottom: 24px;
    }

        .reports-header h2 {
            color: #2d3748;
            margin: 0;
        }

    .subtitle {
        color: #718096;
        margin: 0;
        font-size: 14px;
    }

    .btn {
        padding: 8px 20px;
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

    .stats-grid {
        display: grid;
        grid-template-columns: repeat(auto-fill, minmax(200px, 1fr));
        gap: 16px;
        margin-bottom: 24px;
    }

    .stat-card {
        background: white;
        padding: 20px;
        border-radius: 12px;
        box-shadow: 0 2px 8px rgba(0, 0, 0, 0.06);
        display: flex;
        align-items: center;
        gap: 16px;
        transition: transform 0.2s;
    }

        .stat-card:hover {
            transform: translateY(-2px);
        }

    .stat-icon {
        font-size: 32px;
        width: 50px;
        height: 50px;
        display: flex;
        align-items: center;
        justify-content: center;
        background: #f7fafc;
        border-radius: 12px;
    }

    .stat-content {
        flex: 1;
    }

    .stat-value {
        font-size: 24px;
        font-weight: 700;
        color: #2d3748;
        line-height: 1.2;
    }

    .stat-label {
        font-size: 13px;
        color: #718096;
    }

    .reports-grid {
        display: grid;
        grid-template-columns: 1fr 1fr;
        gap: 20px;
        margin-bottom: 24px;
    }

    .report-card {
        background: white;
        padding: 20px;
        border-radius: 12px;
        box-shadow: 0 2px 8px rgba(0, 0, 0, 0.06);
    }

        .report-card.full-width {
            grid-column: 1 / -1;
        }

        .report-card h3 {
            color: #2d3748;
            margin: 0 0 16px 0;
            font-size: 16px;
        }

    .report-list {
        display: flex;
        flex-direction: column;
        gap: 6px;
    }

    .report-item {
        display: flex;
        align-items: center;
        gap: 12px;
        padding: 8px 12px;
        border-radius: 8px;
        background: #f7fafc;
        transition: background 0.2s;
    }

        .report-item:hover {
            background: #edf2f7;
        }

        .report-item.gold {
            background: #fef3c7;
        }

        .report-item.silver {
            background: #f7fafc;
        }

        .report-item.bronze {
            background: #fef3e2;
        }

    .rank {
        font-weight: 700;
        color: #4a5568;
        min-width: 24px;
        font-size: 14px;
    }

    .report-item.gold .rank {
        color: #d69e2e;
    }

    .report-item.silver .rank {
        color: #a0aec0;
    }

    .report-item.bronze .rank {
        color: #dd6b20;
    }

    .report-item .name {
        flex: 1;
        font-weight: 500;
        color: #2d3748;
    }

    .report-item .rating {
        color: #4a5568;
        font-weight: 600;
        font-size: 14px;
    }

    .report-item .location {
        color: #718096;
        font-size: 13px;
    }

    .distribution {
        display: flex;
        flex-direction: column;
        gap: 10px;
    }

    .distribution-bar {
        display: flex;
        align-items: center;
        gap: 12px;
    }

    .range-label {
        min-width: 40px;
        font-size: 13px;
        font-weight: 600;
        color: #4a5568;
    }

    .bar-track {
        flex: 1;
        height: 24px;
        background: #edf2f7;
        border-radius: 12px;
        overflow: hidden;
        position: relative;
    }

    .bar-fill {
        height: 100%;
        border-radius: 12px;
        transition: width 0.6s ease;
    }

        .bar-fill.bar-red {
            background: #fc8181;
        }

        .bar-fill.bar-orange {
            background: #f6ad55;
        }

        .bar-fill.bar-yellow {
            background: #f6e05e;
        }

        .bar-fill.bar-green {
            background: #68d391;
        }

        .bar-fill.bar-blue {
            background: #63b3ed;
        }

    .count-label {
        min-width: 80px;
        font-size: 13px;
        color: #718096;
        text-align: right;
    }

    .loading-small {
        padding: 20px;
        text-align: center;
    }

    .spinner-small {
        display: inline-block;
        width: 24px;
        height: 24px;
        border: 3px solid #e2e8f0;
        border-top-color: #2d3748;
        border-radius: 50%;
        animation: spin 0.8s linear infinite;
    }

    @keyframes spin {
        to {
            transform: rotate(360deg);
        }
    }

    .empty-small {
        padding: 20px;
        text-align: center;
        color: #a0aec0;
        font-size: 14px;
    }

    .empty-state {
        text-align: center;
        padding: 60px 20px;
        background: white;
        border-radius: 12px;
        margin-top: 20px;
        box-shadow: 0 2px 8px rgba(0, 0, 0, 0.06);
    }

    .empty-icon {
        font-size: 48px;
        margin-bottom: 16px;
    }

    .empty-state h3 {
        color: #2d3748;
        margin-bottom: 8px;
    }

    .empty-state p {
        color: #718096;
    }

    @media (max-width: 768px) {
        .stats-grid {
            grid-template-columns: 1fr 1fr;
        }

        .reports-grid {
            grid-template-columns: 1fr;
        }

        .reports-header {
            flex-direction: column;
            align-items: flex-start;
            gap: 12px;
        }

        .distribution-bar {
            flex-wrap: wrap;
        }

        .count-label {
            min-width: auto;
            text-align: left;
        }
    }

    @media (max-width: 480px) {
        .stats-grid {
            grid-template-columns: 1fr;
        }
    }

    .location-table {
        display: flex;
        flex-direction: column;
        gap: 4px;
    }

    .location-row {
        display: grid;
        grid-template-columns: 2fr 1fr 1fr 1fr;
        padding: 8px 12px;
        border-radius: 8px;
        font-size: 14px;
        gap: 8px;
    }

    .location-header {
        font-weight: 600;
        color: #718096;
        font-size: 12px;
        text-transform: uppercase;
        letter-spacing: 0.05em;
        padding-bottom: 4px;
    }

    .location-row:not(.location-header) {
        background: #f7fafc;
    }

    .location-row:not(.location-header):hover {
        background: #edf2f7;
    }

    .loc-name { font-weight: 500; color: #2d3748; }
    .loc-count, .loc-rating, .loc-reviews { color: #4a5568; }
</style>