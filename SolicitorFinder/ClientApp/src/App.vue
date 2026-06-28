<template>
    <div id="app">
        <header class="header">
            <div class="container">
                <div class="header-content">
                    <h1 class="logo">⚖️ SolicitorFinder</h1>
                    <p class="subtitle">Search and manage solicitor</p>
                </div>
            </div>
        </header>

        <main class="container">
            <div class="tabs">
                <button v-for="tab in tabs"
                        :key="tab.id"
                        class="tab-btn"
                        :class="{ active: activeTab === tab.id }"
                        @click="activeTab = tab.id">
                    <span class="tab-icon">{{ tab.icon }}</span>
                    {{ tab.label }}
                </button>
            </div>

            <div class="tab-content">
                <ConfigTab v-if="activeTab === 'config'" />
                <ListTab v-if="activeTab === 'list'" />
                <ReportsTab v-if="activeTab === 'reports'" />
            </div>
        </main>

        <footer class="footer">
            <div class="container">
                <p>&copy; 2026 SolicitorFinder.</p>
            </div>
        </footer>
    </div>
</template>

<script>
    import { ref } from 'vue'
    import ConfigTab from './components/tabs/ConfigTab.vue'
    import ListTab from './components/tabs/ListTab.vue'
    import ReportsTab from './components/tabs/ReportsTab.vue'

    export default {
        name: 'App',
        components: {
            ConfigTab,
            ListTab,
            ReportsTab
        },
        setup() {
            const activeTab = ref('list')

            const tabs = [
                { id: 'config', label: 'Configuration', icon: '⚙️' },
                { id: 'list', label: 'List', icon: '📋' },
                { id: 'reports', label: 'Reposts', icon: '📊' }
            ]

            return {
                activeTab,
                tabs
            }
        }
    }
</script>

<style>
    * {
        margin: 0;
        padding: 0;
        box-sizing: border-box;
    }

    body {
        font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
        background: #f7fafc;
        color: #2d3748;
        min-height: 100vh;
    }

    .container {
        max-width: 1400px;
        margin: 0 auto;
        padding: 0 20px;
    }

    .header {
        background: linear-gradient(135deg, #2d3748 0%, #4a5568 100%);
        color: white;
        padding: 30px 0;
        box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
    }

    .header-content {
        display: flex;
        justify-content: space-between;
        align-items: center;
        flex-wrap: wrap;
        gap: 12px;
    }

    .logo {
        font-size: 28px;
        font-weight: 700;
    }

    .subtitle {
        font-size: 14px;
        opacity: 0.8;
        font-weight: 300;
    }

    .tabs {
        display: flex;
        gap: 4px;
        margin: 30px 0 24px 0;
        background: white;
        border-radius: 12px;
        padding: 4px;
        box-shadow: 0 2px 8px rgba(0, 0, 0, 0.06);
    }

    .tab-btn {
        flex: 1;
        padding: 12px 24px;
        border: none;
        border-radius: 10px;
        background: transparent;
        font-size: 16px;
        font-weight: 600;
        cursor: pointer;
        transition: all 0.2s;
        color: #718096;
        display: flex;
        align-items: center;
        justify-content: center;
        gap: 8px;
    }

        .tab-btn:hover {
            background: #f7fafc;
            color: #2d3748;
        }

        .tab-btn.active {
            background: #2d3748;
            color: white;
            box-shadow: 0 2px 8px rgba(45, 55, 72, 0.2);
        }

    .tab-icon {
        font-size: 18px;
    }

    .tab-content {
        background: transparent;
        min-height: 400px;
    }

    .footer {
        margin-top: 60px;
        padding: 24px 0;
        background: #2d3748;
        color: #a0aec0;
        text-align: center;
        font-size: 14px;
    }

    @media (max-width: 768px) {
        .header-content {
            flex-direction: column;
            align-items: flex-start;
        }

        .logo {
            font-size: 22px;
        }

        .tabs {
            flex-direction: column;
            background: transparent;
            box-shadow: none;
            gap: 8px;
        }

        .tab-btn {
            background: white;
            justify-content: center;
            padding: 12px;
            border-radius: 10px;
            box-shadow: 0 2px 4px rgba(0, 0, 0, 0.04);
        }

            .tab-btn.active {
                background: #2d3748;
            }
    }
</style>