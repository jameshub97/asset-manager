<script setup lang="ts">
import { useAssetStore } from '@/stores/assetstore'
import { reactive } from 'vue'

const store = useAssetStore()

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

    <button @click="() => store.fetchAssets()">Fetch assets</button>
    <button @click="() => store.fetchAsset('test')">fetch single asset</button>
    <form @submit.prevent="store.createAsset(newAsset)">
    <input v-model="newAsset.name" placeholder="Name" />
    <input v-model="newAsset.description" placeholder="Description" />
    <input v-model.number="newAsset.price" type="number" placeholder="Price" />
    <button type="submit">Create Asset</button>
    </form>


      <button @click="() => store.updateAsset">update asset</button>
      <button @click="() =>store.deleteAsset">delete asset</button>
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
