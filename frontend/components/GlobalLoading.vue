<template>
  <Transition name="fade">
    <div v-if="isLoading" class="optiviera-loading" style="display: flex !important;">
      <div class="optiviera-loading-content">
        <div class="optiviera-spinner">
          <img src="/OptivieraLogo4-Photoroom.png" alt="Optiviera" class="spinner-logo" />
        </div>
        <p class="optiviera-loading-text">{{ loadingText }}</p>
      </div>
    </div>
  </Transition>
</template>

<script setup lang="ts">
const { isLoading, loadingText } = useLoading()

// Debug loading state
watch(isLoading, (newVal) => {
  console.log('Loading state changed:', newVal)
})

watch(loadingText, (newVal) => {
  console.log('Loading text changed:', newVal)
})
</script>

<style scoped>
.optiviera-loading {
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background-color: rgba(5, 68, 94, 0.95);
  display: flex !important;
  justify-content: center;
  align-items: center;
  z-index: 999999;
  transition: opacity 0.5s ease-out;
}

.optiviera-loading-content {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 2rem;
}

.optiviera-spinner {
  width: 150px;
  height: 150px;
  display: flex;
  align-items: center;
  justify-content: center;
  animation: optiviera-spin 2s linear infinite;
  border-radius: 50%;
  box-shadow: 0 0 40px rgba(117, 230, 218, 0.8),
              0 0 80px rgba(24, 154, 180, 0.6);
  filter: drop-shadow(0 0 20px rgba(117, 230, 218, 0.9));
}

.spinner-logo {
  width: 120px;
  height: 120px;
  object-fit: contain;
  animation: optiviera-logo-spin 4s ease-in-out infinite;
}

@keyframes optiviera-spin {
  0% {
    transform: rotate(0deg) scale(1);
  }
  50% {
    transform: rotate(180deg) scale(1.05);
  }
  100% {
    transform: rotate(360deg) scale(1);
  }
}

@keyframes optiviera-logo-spin {
  0% {
    transform: rotate(0deg);
  }
  100% {
    transform: rotate(360deg);
  }
}

.optiviera-loading-text {
  color: white;
  font-size: 1.25rem;
  font-weight: 600;
  text-align: center;
  font-family: 'Montserrat', system-ui, -apple-system, sans-serif;
  letter-spacing: 1px;
  text-shadow: 0 0 10px rgba(117, 230, 218, 0.5);
  animation: pulse-text 2s ease-in-out infinite;
}

@keyframes pulse-text {
  0%, 100% {
    opacity: 1;
  }
  50% {
    opacity: 0.7;
  }
}

/* Fade transition */
.fade-enter-active,
.fade-leave-active {
  transition: opacity 0.5s ease;
}

.fade-enter-from,
.fade-leave-to {
  opacity: 0;
}
</style>
