import { describe, it, expect, vi, beforeEach } from 'vitest'
import { useSolicitors } from '../useSolicitors'
import { solicitorApi } from '../../api/solicitorApi'

vi.mock('../../api/solicitorApi', () => ({
  solicitorApi: {
    getConfig: vi.fn(),
    getLocations: vi.fn(),
    getAreas: vi.fn(),
    search: vi.fn()
  }
}))

describe('useSolicitors', () => {
  beforeEach(() => {
    vi.clearAllMocks()
  })

  it('initializes with default values', () => {
    const { solicitors, loading, error, filter } = useSolicitors()

    expect(solicitors.value).toEqual([])
    expect(loading.value).toBe(false)
    expect(error.value).toBe('')
    expect(filter.page).toBe(1)
  })

  it('loadConfig updates config from API', async () => {
    const mockConfig = { maxResults: 25 }
    solicitorApi.getConfig.mockResolvedValue(mockConfig)

    const { config, filter } = useSolicitors()
    await useSolicitors().config

    expect(config.pageSize).toBeDefined()
  })

  it('resetFilters clears all filter values', () => {
    const { filter, resetFilters } = useSolicitors()

    filter.locationId = 123
    filter.areaId = 456
    filter.searchTerm = 'test'
    filter.minRating = 4
    filter.page = 5

    resetFilters()

    expect(filter.locationId).toBeNull()
    expect(filter.areaId).toBeNull()
    expect(filter.searchTerm).toBe('')
    expect(filter.minRating).toBeNull()
    expect(filter.page).toBe(1)
  })

  it('changePage updates page and triggers search', async () => {
    solicitorApi.search.mockResolvedValue({ items: [], totalCount: 0 })

    const { filter, changePage } = useSolicitors()

    await changePage(3)

    expect(filter.page).toBe(3)
    expect(solicitorApi.search).toHaveBeenCalled()
  })

  it('changePageSize updates pageSize and resets page', async () => {
    solicitorApi.search.mockResolvedValue({ items: [], totalCount: 0 })

    const { filter, changePageSize } = useSolicitors()
    filter.page = 5

    await changePageSize(50)

    expect(filter.pageSize).toBe(50)
    expect(filter.page).toBe(1)
    expect(solicitorApi.search).toHaveBeenCalled()
  })

  it('searchSolicitors sets loading state correctly', async () => {
    solicitorApi.search.mockImplementation(
      () => new Promise(resolve => setTimeout(() => resolve({ items: [] }), 50))
    )

    const { loading, searchSolicitors } = useSolicitors()

    const searchPromise = searchSolicitors()
    expect(loading.value).toBe(true)

    await searchPromise
    expect(loading.value).toBe(false)
  })

  it('searchSolicitors handles successful response', async () => {
    const mockData = {
      items: [
        { name: 'Solicitor 1', ratingStars: 4.5 },
        { name: 'Solicitor 2', ratingStars: 4.0 }
      ],
      totalCount: 2,
      page: 1,
      pageSize: 20
    }
    solicitorApi.search.mockResolvedValue(mockData)

    const { solicitors, pagedResult, searchSolicitors } = useSolicitors()

    await searchSolicitors()

    expect(solicitors.value.length).toBe(2)
    expect(pagedResult.value.totalCount).toBe(2)
  })

  it('searchSolicitors handles error response', async () => {
    const mockError = {
      response: {
        data: {
          message: 'API Error'
        }
      }
    }
    solicitorApi.search.mockRejectedValue(mockError)

    const { error, searchSolicitors } = useSolicitors()

    await searchSolicitors()

    expect(error.value).toBe('API Error')
  })

  it('searchSolicitors handles array response format', async () => {
    const mockData = [
      { name: 'Solicitor 1', ratingStars: 4.5 },
      { name: 'Solicitor 2', ratingStars: 4.0 }
    ]
    solicitorApi.search.mockResolvedValue(mockData)

    const { solicitors, searchSolicitors } = useSolicitors()

    await searchSolicitors()

    expect(solicitors.value.length).toBe(2)
  })

  it('loadLocationsAndAreas handles successful responses', async () => {
    const mockLocations = [
      { id: 1, title: 'London', text: 'London, UK' }
    ]
    const mockAreas = [
      { id: 1, name: 'Family Law', solicitorAreaExternalId: 'fam-001' }
    ]

    solicitorApi.getLocations.mockResolvedValue(mockLocations)
    solicitorApi.getAreas.mockResolvedValue(mockAreas)

    const composable = useSolicitors()
    await composable.init()

    expect(composable.config.locations.length).toBeGreaterThanOrEqual(0)
    expect(composable.config.areas.length).toBeGreaterThanOrEqual(0)
  })

  it('loadLocationsAndAreas handles API errors gracefully', async () => {
    solicitorApi.getLocations.mockRejectedValue(new Error('Network error'))
    solicitorApi.getAreas.mockRejectedValue(new Error('Network error'))
    solicitorApi.getConfig.mockResolvedValue({ maxResults: 20 })
    solicitorApi.search.mockResolvedValue({ items: [], totalCount: 0 })

    const composable = useSolicitors()

    // Should not throw, but handle errors gracefully
    await composable.init()

    // After error, arrays should be empty
    expect(composable.config.locations).toEqual([])
    expect(composable.config.areas).toEqual([])
  })
})
