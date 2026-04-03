<script setup lang="ts">
import { useAssetStore } from '@/stores/assetstore'
import { reactive } from 'vue'
import type { Asset } from '@/services/api'

const props = defineProps<{
  asset: Asset
}>()

const emit = defineEmits<{
  close: []
}>()

const store = useAssetStore()

const formData = reactive({
  name: props.asset.name,
  description: props.asset.description,
  price: props.asset.price || 0,
})

const handleSubmit = async () => {
  if (!props.asset.id) return

  try {
    await store.updateAsset(props.asset.id, {
      name: formData.name,
      description: formData.description,
      price: formData.price,
    })
    emit('close')
  } catch (err) {
    console.error('Update failed:', err)
  }
}

const handleCancel = () => {
  emit('close')
}
</script>

<template>
  <div class="modal-overlay" @click.self="handleCancel">
    <div class="modal-content">
      <div class="modal-header">
        <h2 class="green">Update Asset</h2>
        <button class="close-btn" @click="handleCancel">✕</button>
      </div>

      <form @submit.prevent="handleSubmit" class="update-form">
        <div class="form-group">
          <label for="name">Asset Name</label>
          <input v-model="formData.name" id="name" type="text" placeholder="Asset name" />
        </div>

        <div class="form-group">
          <label for="description">Description</label>
          <textarea
            v-model="formData.description"
            id="description"
            placeholder="Asset description"
            rows="3"
          ></textarea>
        </div>

        <div class="form-group">
          <label for="price">Price</label>
          <input
            v-model.number="formData.price"
            id="price"
            type="number"
            placeholder="0.00"
            min="0"
            step="0.01"
          />
        </div>

        <div class="modal-actions">
          <button type="button" class="btn-cancel" @click="handleCancel">Cancel</button>
          <button type="submit" class="btn-submit" :disabled="store.loading">
            {{ store.loading ? 'Updating...' : 'Update Asset' }}
          </button>
        </div>
      </form>

      <div v-if="store.error" class="error-message">
        {{ store.error }}
      </div>
    </div>
  </div>
</template>

<style scoped>
.modal-overlay {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: rgba(0, 0, 0, 0.5);
  display: flex;
  justify-content: center;
  align-items: center;
  z-index: 1000;
}

.modal-content {
  background: white;
  border-radius: 12px;
  box-shadow: 0 10px 40px rgba(0, 0, 0, 0.2);
  max-width: 500px;
  width: 90%;
  padding: 2rem;
  animation: slideIn 0.3s ease-out;
}

@keyframes slideIn {
  from {
    transform: translateY(-20px);
    opacity: 0;
  }
  to {
    transform: translateY(0);
    opacity: 1;
  }
}

.modal-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 1.5rem;
  padding-bottom: 1rem;
  border-bottom: 1px solid rgba(66, 184, 131, 0.2);
}

.modal-header h2 {
  margin: 0;
  font-size: 1.5rem;
}

.close-btn {
  background: none;
  border: none;
  font-size: 1.5rem;
  cursor: pointer;
  color: #9ca3af;
  padding: 0;
  transition: color 0.2s;
}

.close-btn:hover {
  color: #6b7280;
}

.green {
  color: #42b883;
}

.update-form {
  display: flex;
  flex-direction: column;
  gap: 1.25rem;
  margin-bottom: 1.5rem;
}

.form-group {
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
}

.form-group label {
  font-weight: 600;
  color: #374151;
  font-size: 0.875rem;
}

.form-group input,
.form-group textarea {
  padding: 10px 12px;
  border: 1px solid #cbd5e1;
  border-radius: 6px;
  font-size: 14px;
  font-family: inherit;
  transition: all 0.2s;
}

.form-group input:focus,
.form-group textarea:focus {
  outline: none;
  border-color: #42b883;
  box-shadow: 0 0 0 3px rgba(66, 184, 131, 0.1);
}

.form-group textarea {
  resize: vertical;
}

.modal-actions {
  display: flex;
  gap: 1rem;
  justify-content: flex-end;
}

.btn-cancel,
.btn-submit {
  padding: 10px 20px;
  border: none;
  border-radius: 6px;
  cursor: pointer;
  font-size: 14px;
  font-weight: 500;
  transition: all 0.2s;
}

.btn-cancel {
  background: #e5e7eb;
  color: #374151;
}

.btn-cancel:hover {
  background: #d1d5db;
}

.btn-submit {
  background: #42b883;
  color: white;
}

.btn-submit:hover:not(:disabled) {
  background: #33a06f;
}

.btn-submit:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}

.error-message {
  background: #fee2e2;
  border: 1px solid #fecaca;
  color: #dc2626;
  padding: 0.75rem 1rem;
  border-radius: 6px;
  font-size: 0.875rem;
}
</style>
