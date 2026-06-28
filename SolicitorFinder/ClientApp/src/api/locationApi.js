import axios from 'axios'

const API_URL = '/api/location/search'

export const locationApi = {
    searchLocations: async (query) => {
        if (!query || query.length < 2) {
            return []
        }

        try {
            const response = await axios.get(API_URL, {
                params: { searchText: query },
                timeout: 10000
            })

            return response.data || []
        } catch (error) {
            console.error('Error searching locations:', error)
            return []
        }
    }
}