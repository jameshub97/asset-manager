<script setup lang="ts">
import { useAssetStore } from '@/stores/assetstore'
import { reactive } from 'vue'
import { auth } from '@/services/auth'
import { useToast } from 'vue-toastification'

const store = useAssetStore()
const isGuest = auth.isGuest()
const toast = useToast()

const newAsset = reactive({
  name: '',
  description: '',
  price: 0,
  userId: localStorage.getItem('userId') || '',
})

const handleCreate = async () => {
  if (isGuest) return
  try {
    await store.createAsset(newAsset)
    newAsset.name = ''
    newAsset.description = ''
    newAsset.price = 0
    newAsset.userId = localStorage.getItem('userId') || ''
    toast.success('Asset created successfully.')
  } catch (err: unknown) {
    const message = err instanceof Error ? err.message : 'Failed to create asset.'
    toast.error(message)
  }
}
</script>

<template>
  <div class="create-asset">
    <h2 class="green">Create an asset</h2>
    <p v-if="isGuest" class="guest-note">Guest mode: creating assets is disabled.</p>
    <form @submit.prevent="handleCreate">
      <input v-model="newAsset.name" placeholder="Name" :disabled="isGuest || store.loading" />
      <input v-model="newAsset.description" placeholder="Description" :disabled="isGuest || store.loading" />
      <input
        v-model.number="newAsset.price"
        type="number"
        placeholder="Price"
        min="0"
        step="0.01"
        :disabled="isGuest || store.loading"
      />
      <button type="submit" :disabled="isGuest || store.loading">Create Asset</button>
    </form>
  </div>
</template>

<style scoped>
.create-asset {
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

h2 {
  margin: 0;
}

.guest-note {
  margin: 0;
  padding: 8px 12px;
  border-radius: 6px;
  background: #fffbeb;
  border: 1px solid #fde68a;
  color: #92400e;
  font-size: 0.875rem;
}

form {
  display: flex;
  flex-direction: column;
  gap: 8px;
}

input {
  padding: 8px 12px;
  border: 1px solid #cbd5e1;
  border-radius: 6px;
  font-size: 14px;
}

input:focus {
  outline: none;
  border-color: #42b883;
  box-shadow: 0 0 0 2px rgba(66, 184, 131, 0.2);
}

button {
  padding: 8px 16px;
  background: #42b883;
  color: white;
  border: none;
  border-radius: 6px;
  cursor: pointer;
  font-size: 14px;
  font-weight: 500;
  transition: background 0.2s;
}

button:hover {
  background: #33a06f;
}

button:disabled {
  background: #94a3b8;
  cursor: not-allowed;
}
</style>
