export class Config {
    constructor(data = {}) {
        this.updateInterval = data.updateInterval || 60
        this.autoUpdate = data.autoUpdate ?? true
        this.maxResults = data.maxResults || 20
        this.locations = (data.locations || []).map(l => ({
            title: l.title || '',
            text: l.text || ''
        }))
    }
}