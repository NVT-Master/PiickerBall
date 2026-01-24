<template>
  <Teleport to="body">
    <div 
      v-if="modelValue" 
      class="modal fade show d-block" 
      tabindex="-1"
      @click.self="handleBackdropClick"
    >
      <div class="modal-dialog" :class="[sizeClass, { 'modal-dialog-centered': centered }]">
        <div class="modal-content">
          <!-- Header -->
          <div v-if="$slots.header || title" class="modal-header">
            <slot name="header">
              <h5 class="modal-title">{{ title }}</h5>
            </slot>
            <button 
              v-if="showClose"
              type="button" 
              class="btn-close" 
              @click="close"
            ></button>
          </div>

          <!-- Body -->
          <div class="modal-body">
            <slot></slot>
          </div>

          <!-- Footer -->
          <div v-if="$slots.footer" class="modal-footer">
            <slot name="footer"></slot>
          </div>
        </div>
      </div>
    </div>
    <div v-if="modelValue" class="modal-backdrop fade show"></div>
  </Teleport>
</template>

<script setup>
import { computed, watch } from 'vue'

const props = defineProps({
  modelValue: {
    type: Boolean,
    default: false
  },
  title: {
    type: String,
    default: ''
  },
  size: {
    type: String,
    default: 'md',
    validator: (value) => ['sm', 'md', 'lg', 'xl'].includes(value)
  },
  centered: {
    type: Boolean,
    default: true
  },
  showClose: {
    type: Boolean,
    default: true
  },
  closeOnBackdrop: {
    type: Boolean,
    default: true
  }
})

const emit = defineEmits(['update:modelValue', 'close'])

const sizeClass = computed(() => {
  if (props.size === 'md') return ''
  return `modal-${props.size}`
})

function close() {
  emit('update:modelValue', false)
  emit('close')
}

function handleBackdropClick() {
  if (props.closeOnBackdrop) {
    close()
  }
}

// Handle body scroll
watch(() => props.modelValue, (value) => {
  if (value) {
    document.body.classList.add('modal-open')
    document.body.style.overflow = 'hidden'
  } else {
    document.body.classList.remove('modal-open')
    document.body.style.overflow = ''
  }
})
</script>

<style scoped>
.modal.show {
  background-color: rgba(0, 0, 0, 0.5);
}
</style>
