import axios from 'axios'

const api = axios.create({
    baseURL: '/api',
    headers: {
        'Content-Type': 'application/json'
    }
})

export const solicitorApi = {
    search: async (params) => {
        const response = await api.get('/solicitors', { params })
        return response.data
    },

    getLocations: async () => {
        const response = await api.get('/location/locations')
        return response.data
    },

    getAreas: async () => {
        const response = await api.get('/area')
        return response.data
    },


    getById: async (id) => {
        const response = await api.get(`/solicitors/${id}`)
        return response.data
    },

    getBySid: async (sid) => {
        const response = await api.get(`/solicitors/sid/${sid}`)
        return response.data
    },

    getList: async (params) => {
        const response = await api.get('/solicitors', { params })
        return response.data
    },

    updateConfig: async (config) => {
        const response = await api.put('/configuration', {
            updateInterval: config.updateInterval,
            autoUpdate: config.autoUpdate,
            maxResults: config.maxResults,
            locations: config.locations || []
        })
        return response.data
    },

    getConfig: async () => {
        const response = await api.get('/configuration')
        return response.data
    },

    getReports: async (topCount = 5)  => {
        const response = await fetch(`/api/solicitors/reports?topCount=${topCount}`)
        if (!response.ok) {
            throw new Error('Failed to fetch reports')
        }
        return response.json()
    }
}