<template>
    <div class="solicitor-card">
        <div class="card-header">
            <h3 class="name">{{ solicitor.name }}</h3>
        </div>

        <div class="rating-section">
            <!-- ✅ Генерируем звёзды -->
            <span class="stars">{{ getStars(solicitor.ratingStars) }}</span>
            <span class="rating-value">{{ solicitor.ratingStars.toFixed(1) }}</span>
            <span class="review-count">({{ solicitor.reviewCount }} reviews)</span>
        </div>

        <div class="location" v-if="solicitor.location">
            📍 {{ solicitor.location }}
        </div>

        <div class="details">
            <div v-if="solicitor.phone" class="detail-item">
                📞 <a :href="'tel:' + solicitor.phone">{{ solicitor.phone }}</a>
            </div>
            <div v-if="solicitor.emailLink" class="detail-item">
                ✉️ <a :href="solicitor.emailLink">{{ solicitor.emailLink }}</a>
            </div>
            <div v-if="solicitor.website" class="detail-item">
                🌐 <a :href="solicitor.website" target="_blank" rel="noopener">{{ solicitor.website }}</a>
            </div>
            <div v-if="solicitor.address" class="detail-item">
                🏠 {{ solicitor.address }}
            </div>
        </div>

        <div class="description" v-if="solicitor.description">
            {{ solicitor.description }}
        </div>

        <div class="card-footer">
            <span class="date">Updated: {{ formatDate(solicitor.scrapedAt) }}</span>
        </div>
    </div>
</template>

<script>
    export default {
        name: 'SolicitorCard',
        props: {
            solicitor: {
                type: Object,
                required: true
            }
        },
        methods: {
            getStars(rating) {
                const full = Math.floor(rating)
                const half = rating - full >= 0.5 ? 1 : 0
                const empty = 5 - full - half

                return '★'.repeat(full) + (half ? '½' : '') + '☆'.repeat(empty)
            },
            formatDate(date) {
                if (!date) return '—'
                const d = new Date(date)
                return d.toLocaleDateString('En-us', {
                    day: '2-digit',
                    month: '2-digit',
                    year: 'numeric',
                    hour: '2-digit',
                    minute: '2-digit'
                })
            }
        }
    }
</script>

<style scoped>
    .solicitor-card {
        background: white;
        border-radius: 12px;
        padding: 20px;
        box-shadow: 0 2px 8px rgba(0, 0, 0, 0.06);
        border: 1px solid #edf2f7;
        transition: all 0.2s;
    }

        .solicitor-card:hover {
            box-shadow: 0 4px 16px rgba(0, 0, 0, 0.1);
            transform: translateY(-2px);
        }

    .card-header {
        display: flex;
        justify-content: space-between;
        align-items: flex-start;
        margin-bottom: 8px;
    }

    .name {
        font-size: 18px;
        font-weight: 700;
        color: #2d3748;
        margin: 0;
    }

    .rating-section {
        display: flex;
        align-items: center;
        gap: 8px;
        margin-bottom: 10px;
    }

    .stars {
        color: #f6ad55;
        font-size: 18px;
        letter-spacing: 1px;
    }

    .rating-value {
        font-weight: 600;
        color: #2d3748;
    }

    .review-count {
        color: #a0aec0;
        font-size: 14px;
    }

    .location {
        color: #4a5568;
        font-size: 14px;
        margin-bottom: 10px;
        padding: 4px 0;
        border-bottom: 1px solid #f7fafc;
    }

    .details {
        display: flex;
        flex-direction: column;
        gap: 4px;
        margin: 8px 0;
    }

    .detail-item {
        font-size: 14px;
        color: #4a5568;
        word-break: break-all;
    }

        .detail-item a {
            color: #2d3748;
            text-decoration: none;
        }

            .detail-item a:hover {
                color: #4a5568;
                text-decoration: underline;
            }

    .description {
        margin-top: 10px;
        padding-top: 10px;
        border-top: 1px solid #edf2f7;
        color: #4a5568;
        font-size: 14px;
        line-height: 1.5;
        display: -webkit-box;
        -webkit-line-clamp: 3;
        -webkit-box-orient: vertical;
        overflow: hidden;
    }

    .card-footer {
        margin-top: 10px;
        padding-top: 10px;
        border-top: 1px solid #edf2f7;
    }

    .date {
        font-size: 12px;
        color: #a0aec0;
    }
</style>