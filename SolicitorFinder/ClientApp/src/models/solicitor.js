export class Solicitor {
    constructor(data = {}) {
        this.id = data.id || 0
        this.name = data.name || ''
        this.phone = data.phone || ''
        this.address = data.address || ''
        this.description = data.description || ''
        this.website = data.website || ''
        this.emailLink = data.emailLink || ''
        this.ratingStars = data.ratingStars || 0
        this.reviewCount = data.reviewCount || 0
        this.location = data.location || ''
        this.areaId = data.areaId || ''
        this.scrapedAt = data.scrapedAt || new Date().toISOString()
        this.createdAt = data.createdAt || new Date().toISOString()
        this.updatedAt = data.updatedAt || null
    }
}

export class SolicitorFilter {
    constructor(data = {}) {
        this.locationId = data.locationId || null
        this.areaId = data.areaId || null
        this.searchTerm = data.searchTerm || ''
        this.minRating = data.minRating || null
        this.maxRating = data.maxRating || null
        this.minReviewCount = data.minReviewCount || null
        this.maxReviewCount = data.maxReviewCount || null
        this.page = data.page || 1
        this.pageSize = data.pageSize || 20
        this.sortBy = data.sortBy || 'RatingStars'
        this.sortDescending = data.sortDescending ?? true
    }

    toQueryParams() {
        const params = {}
        if (this.locationId) params.locationId = this.locationId
        if (this.areaId) params.areaId = this.areaId
        if (this.searchTerm) params.searchTerm = this.searchTerm
        if (this.minRating !== null) params.minRating = this.minRating
        if (this.maxRating !== null) params.maxRating = this.maxRating
        if (this.minReviewCount !== null) params.minReviewCount = this.minReviewCount
        if (this.maxReviewCount !== null) params.maxReviewCount = this.maxReviewCount
        params.page = this.page
        params.pageSize = this.pageSize
        params.sortBy = this.sortBy
        params.sortDescending = this.sortDescending
        return params
    }
}

export class PagedResult {
    constructor(data = {}) {
        this.items = (data.items || []).map(item => new Solicitor(item))
        this.totalCount = data.totalCount || 0
        this.page = data.page || 1
        this.pageSize = data.pageSize || 20
    }

    get totalPages() {
        return Math.ceil(this.totalCount / this.pageSize)
    }

    get hasNext() {
        return this.page < this.totalPages
    }

    get hasPrevious() {
        return this.page > 1
    }
}