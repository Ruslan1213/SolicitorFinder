<template>
    <div class="pagination" v-if="totalPages > 1">
        <button class="pagination-btn"
                :disabled="!hasPrevious"
                @click="$emit('page-change', currentPage - 1)">
            ◀
        </button>

        <button v-for="page in visiblePages"
                :key="page"
                class="pagination-btn"
                :class="{ active: page === currentPage }"
                @click="$emit('page-change', page)">
            {{ page }}
        </button>

        <button class="pagination-btn"
                :disabled="!hasNext"
                @click="$emit('page-change', currentPage + 1)">
            ▶
        </button>

        <span class="pagination-info">
            {{ totalCount }} Items, Page {{ currentPage }} - {{ totalPages }}
        </span>
    </div>
</template>

<script>
export default {
    name: 'Pagination',
    props: {
        currentPage: {
            type: Number,
            required: true
        },
        totalPages: {
            type: Number,
            required: true
        },
        totalCount: {
            type: Number,
            default: 0
        },
        maxVisible: {
            type: Number,
            default: 5
        }
    },
    emits: ['page-change'],
    computed: {
        hasPrevious() {
            return this.currentPage > 1
        },
        hasNext() {
            return this.currentPage < this.totalPages
        },
        visiblePages() {
            const pages = []
            const start = Math.max(1, this.currentPage - Math.floor(this.maxVisible / 2))
            const end = Math.min(this.totalPages, start + this.maxVisible - 1)

            for (let i = start; i <= end; i++) {
                pages.push(i)
            }
            return pages
        }
    }
}
</script>

<style scoped>
    .pagination {
        display: flex;
        align-items: center;
        gap: 8px;
        padding: 16px 0;
        flex-wrap: wrap;
    }

    .pagination-btn {
        padding: 8px 14px;
        border: 1px solid #e2e8f0;
        border-radius: 8px;
        background: white;
        cursor: pointer;
        transition: all 0.2s;
        font-size: 14px;
        color: #2d3748;
    }

        .pagination-btn:hover:not(:disabled) {
            background: #edf2f7;
            border-color: #4a5568;
        }

        .pagination-btn.active {
            background: #2d3748;
            color: white;
            border-color: #2d3748;
        }

        .pagination-btn:disabled {
            opacity: 0.4;
            cursor: not-allowed;
        }

    .pagination-info {
        margin-left: 12px;
        color: #718096;
        font-size: 14px;
    }
</style>