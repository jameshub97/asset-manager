<script setup lang="ts">
import { useAssetStore } from '@/stores/assetstore'
import { reactive, ref } from 'vue'

const store = useAssetStore()
const assetId = ref('')  // User inputs ID here

const newAsset = reactive({
  name: '',
  description: '',
  price: 0
})

defineProps<{
  msg: string
}>()
</script>

<template>
  <div class="greetings">
    <h1 class="green">{{ msg }}</h1>
    <h3>
  <div class="button-group">

    <!-- lookup  data -->
    <!-- Fetch by ID form -->
    <form @submit.prevent="assetId && store.fetchAsset(assetId)">
      <input
        v-model="assetId"
        placeholder="Enter asset ID"
        required
      />
      <button type="submit">Fetch Asset</button>
    </form>

    <button @click="() => store.fetchAsset('1')">Fetch single asset</button>
    <button @click="() => store.fetchAssets()">Fetch all assets</button>


    <!-- forum for data -->
    <form @submit.prevent="store.createAsset(newAsset)">
    <input v-model="newAsset.name" placeholder="Name" />
    <input v-model="newAsset.description" placeholder="Description" />
    <input v-model.number="newAsset.price" type="number" placeholder="Price" />
    <button type="submit">Create Asset</button>
    </form>

    <div v-if="store.selectedAsset">
      <h2>{{ store.selectedAsset.name }}</h2>
      <p>{{ store.selectedAsset.description }}</p>
      <p>Price: ${{ store.selectedAsset.price }}</p>
      <p>ID: {{ store.selectedAsset.id }}</p>
    </div>
<div v-if="store.assets.length">
  <ul>
<li v-for="asset in store.assets" :key="asset.id">
  {{ asset.name }} - ${{ asset.price }}
  <button
    @click="asset.id && store.fetchAsset(asset.id)"
    :disabled="!asset.id"
  >
    View
  </button>
</li>
  </ul>
</div>
<p v-else>No assets loaded</p>
  </div>


    </h3>
  </div>
</template>

<style scoped>
h1 {
  font-weight: 500;
  font-size: 2.6rem;
  position: relative;
  top: -10px;
}

h3 {
  font-size: 1.2rem;
}

.greetings h1,
.greetings h3 {
  text-align: center;
}

.button-group {
  display: flex;
  flex-direction: column;  /* Stack vertically */
  gap: 8px;                /* Space between buttons */
  align-items: flex-start; /* Align left (or center/stretch) */
}

@media (min-width: 1024px) {
  .greetings h1,
  .greetings h3 {
    text-align: left;
  }
}
</style>
