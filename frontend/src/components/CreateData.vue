<script setup lang="ts">
import { useAssetStore } from '@/stores/assetstore'

const store = useAssetStore()

defineProps<{
  msg: string
}>()
</script>

<template>
  <div class="greetings">
    <h1 class="green">{{ msg }}</h1>
    <h3>
      <button @click="() => store.fetchAssets">create new item!</button>
      <button @click="() => store.fetchAsset">fetch single item</button>
      <button @click="() => store.createAsset">fetch single item</button>
      <button @click="() => store.updateAsset">fetch single item</button>
      <button @click="() =>store.deleteAsset">fetch single item</button>
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

@media (min-width: 1024px) {
  .greetings h1,
  .greetings h3 {
    text-align: left;
  }
}
</style>
