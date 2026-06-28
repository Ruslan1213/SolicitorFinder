import { ref, reactive } from 'vue'
import { solicitorApi } from '../api/solicitorApi.js'
import { SolicitorFilter, PagedResult, Solicitor } from '../models/solicitor.js'

export function useSolicitors() {
    const solicitors = ref([])
    const pagedResult = ref(new PagedResult())
    const loading = ref(false)
    const error = ref('')
    const filter = reactive(new SolicitorFilter())

    const config = reactive({
        pageSize: 20,
        locations: [],
        areas: []
    })

    const loadConfig = async () => {
        try {
            const data = await solicitorApi.getConfig()
            if (data?.maxResults) {
                config.pageSize = data.maxResults
                filter.pageSize = data.maxResults
            }
        } catch (err) {
            console.error('Failed to load config:', err)
        }
    }

    const loadLocationsAndAreas = async () => {
        try {
            const [locations, areas] = await Promise.all([
                solicitorApi.getLocations(),
                solicitorApi.getAreas()
            ])

            config.locations = (locations || []).map(l => ({
                id: l.id,
                name: l.title || '',
                title: l.title || '',
                text: l.text || ''
            }))

            config.areas = (areas || []).map(a => ({
                id: a.id,
                name: a.name || '',
                solicitorAreaExternalId: a.solicitorAreaExternalId || ''
            }))

        } catch (err) {
            console.error('Failed to load locations/areas:', err)
            config.locations = []
            config.areas = []
        }
    }

    const searchSolicitors = async () => {
        loading.value = true
        error.value = ''
        solicitors.value = []

        try {
            const params = filter.toQueryParams()
            const data = await solicitorApi.search(params)

            if (data?.items) {
                pagedResult.value = new PagedResult(data)
                solicitors.value = pagedResult.value.items
            } else if (Array.isArray(data)) {
                solicitors.value = data.map(item => new Solicitor(item))
                pagedResult.value = new PagedResult({
                    items: data,
                    totalCount: data.length,
                    page: filter.page,
                    pageSize: filter.pageSize
                })
            } else {
                pagedResult.value = new PagedResult(data || {})
                solicitors.value = pagedResult.value.items
            }
        } catch (err) {
            error.value = err.response?.data?.message || 'Search error'
            console.error('Search error:', err)
        } finally {
            loading.value = false
        }
    }

    const resetFilters = () => {
        filter.locationId = null
        filter.areaId = null
        filter.searchTerm = ''
        filter.minRating = null
        filter.maxRating = null
        filter.minReviewCount = null
        filter.maxReviewCount = null
        filter.page = 1
        filter.pageSize = config.pageSize || 20
        solicitors.value = []
        error.value = ''
    }

    const changePage = (page) => {
        filter.page = page
        searchSolicitors()
    }

    const changePageSize = (size) => {
        filter.pageSize = size
        filter.page = 1
        searchSolicitors()
    }

    const init = async () => {
        await Promise.all([
            loadConfig(),
            loadLocationsAndAreas()
        ])
    }

    return {
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
    }
}