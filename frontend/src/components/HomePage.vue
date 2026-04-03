<script setup lang="ts">
import { useAssetStore } from '@/stores/assetstore'
import { useRouter } from 'vue-router'
import { auth } from '@/services/auth'
import { onMounted } from 'vue'
import AssetList from './AssetList.vue'
import AssetLookup from './AssetLookup.vue'
import AssetOptions from './AssetOptions.vue'
import CreateAsset from './CreateAsset.vue'

const store = useAssetStore()
const router = useRouter()

onMounted(() => {
  store.fetchAssets()
})

const handleLogout = () => {
  auth.logout()
  router.push('/login')
}
</script>

<template>
  <div class="home-container">
    <div class="content-card">
      <div class="header">
        <div>
          <h1 class="green">Asset Manager</h1>
          <p v-if="auth.isGuest()" class="guest-badge">👤 Guest User</p>
        </div>
        <button class="signout-btn" @click="handleLogout">Sign Out</button>
      </div>

      <div class="content-wrapper">
        <div class="left-panel">
          <div class="left-grid">
            <CreateAsset/>
            <AssetOptions />
            <AssetLookup />
          </div>
        </div>
        <div class="right-panel">
          <AssetList />
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
.home-container {
  display: flex;
  flex-direction: column;
  padding: 1.5rem;
  min-height: 100vh;
  background: #f9fafb;
  justify-content: center;
  align-items: center;
}

.content-card {
  display: flex;
  flex-direction: column;
  gap: 1.5rem;
  background: white;
  border-radius: 12px;
  padding: 2rem;
  box-shadow: 0 4px 6px rgba(0, 0, 0, 0.07);
  width: 100%;
  max-width: 1400px;
}

.header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding-bottom: 1rem;
  border-bottom: 1px solid rgba(66, 184, 131, 0.2);
}

.header > div {
  display: flex;
  align-items: center;
  gap: 1rem;
}

.green {
  color: #42b883;
}

h1 {
  font-weight: 500;
  font-size: 2.6rem;
  margin: 0;
}

.guest-badge {
  margin: 0;
  font-size: 0.875rem;
  color: #6b7280;
  font-weight: 500;
}

.signout-btn {
  padding: 8px 16px;
  background: #dc2626;
  color: white;
  border: none;
  border-radius: 6px;
  cursor: pointer;
  font-size: 14px;
  font-weight: 500;
  transition: background 0.2s;
}

.signout-btn:hover {
  background: #991b1b;
}

.content-wrapper {
  display: flex;
  gap: 2rem;
  flex: 1;
}

.left-panel {
  flex: 1;
  display: flex;
  flex-direction: column;
  min-height: 0;
}

.left-grid {
  display: grid;
  grid-template-rows: auto auto auto;
  gap: 1.5rem;
}

.left-grid > * {
  padding: 1.5rem;
  background: linear-gradient(135deg, rgba(66, 184, 131, 0.1) 0%, rgba(51, 160, 111, 0.05) 100%);
  border-radius: 8px;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.1);
  border: 1px solid rgba(66, 184, 131, 0.2);
}

.right-panel {
  flex: 1;
  display: flex;
  flex-direction: column;
  min-height: 0;
}

h3 {
  font-size: 1.2rem;
}

.greetings h1,
.greetings h3 {
  text-align: center;
}

@media (max-width: 768px) {
  .header {
    flex-direction: column;
    gap: 1rem;
    align-items: flex-start;
  }

  .content-wrapper {
    flex-direction: column;
    gap: 1.5rem;
    flex: auto;
  }

  .left-grid {
    grid-template-rows: auto auto auto;
  }

  .left-panel,
  .right-panel {
    flex: auto;
  }
}

@media (min-width: 1024px) {
  .greetings h1,
  .greetings h3 {
    text-align: left;
  }
}
</style>
