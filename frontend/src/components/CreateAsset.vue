<script setup lang="ts">
import { useAssetStore } from '@/stores/assetstore'
import { reactive } from 'vue'

const store = useAssetStore()

const newAsset = reactive({
  name: '',
  description: '',
  price: 0,
})
</script>

<template>
  <div class="create-asset">
    <h2 class="green">Create an asset</h2>
    <form @submit.prevent="store.createAsset(newAsset)">
      <input v-model="newAsset.name" placeholder="Name" />
      <input v-model="newAsset.description" placeholder="Description" />
      <input v-model.number="newAsset.price" type="number" placeholder="Price" />
      <button type="submit">Create Asset</button>
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
