<template>
  <teleport to="body">
    <div class="modal-backdrop fade show" @click="$emit('close')"></div>
    <div class="modal fade show d-block" tabindex="-1">
      <div class="modal-dialog modal-dialog-centered" :class="modalClass">
        <div class="modal-content">
          <div class="modal-header" v-if="!hideHeader">
            <h5 class="modal-title">
              <slot name="title">{{ title }}</slot>
            </h5>
            <button 
              type="button" 
              class="btn-close" 
              @click="$emit('close')"
              :disabled="loading"
            ></button>
          </div>
          
          <div class="modal-body">
            <slot></slot>
          </div>
          
          <div class="modal-footer" v-if="!hideFooter">
            <slot name="footer">
              <button 
                type="button" 
                class="btn btn-secondary" 
                @click="$emit('close')"
                :disabled="loading"
              >
                {{ cancelText }}
              </button>
              <button 
                type="button" 
                class="btn" 
                :class="confirmClass || 'btn-primary'"
                @click="$emit('confirm')"
                :disabled="loading"
              >
                <span v-if="loading" class="spinner-border spinner-border-sm me-2"></span>
                {{ confirmText }}
              </button>
            </slot>
          </div>
        </div>
      </div>
    </div>
  </teleport>
</template>

<script setup>
import { computed, onMounted, onUnmounted } from 'vue'

const props = defineProps({
  title: {
    type: String,
    default: ''
  },
  size: {
    type: String,
    default: '', // '', 'sm', 'lg', 'xl'
    validator: (v) => ['', 'sm', 'lg', 'xl'].includes(v)
  },
  hideHeader: Boolean,
  hideFooter: Boolean,
  loading: Boolean,
  cancelText: {
    type: String,
    default: 'Hủy'
  },
  confirmText: {
    type: String,
    default: 'Xác nhận'
  },
  confirmClass: {
    type: String,
    default: ''
  }
})

const emit = defineEmits(['close', 'confirm'])

const modalClass = computed(() => {
  return props.size ? `modal-${props.size}` : ''
})

// Prevent body scroll when modal is open
onMounted(() => {
  document.body.classList.add('modal-open')
})

onUnmounted(() => {
  document.body.classList.remove('modal-open')
})
</script>

<style scoped>
.modal-backdrop {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background-color: rgba(0, 0, 0, 0.5);
  z-index: 1040;
}

.modal {
  z-index: 1050;
}
</style>
