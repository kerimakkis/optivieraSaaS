<template>
  <div>
    <GlobalLoading />
    <NuxtPage />
  </div>
</template>

<script setup lang="ts">
const { start, finish } = useLoading()

// Show loading when app starts
start('Loading Optiviera...')

// Hide loading when app is mounted
onMounted(() => {
  // Simulate initial app load
  setTimeout(() => {
    finish()
  }, 2000) // Increased to 2 seconds to see the loading
})
</script>

<style>
body {
  margin: 0;
  font-family: 'Montserrat', 'Inter', -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, sans-serif;
  background-color: var(--optiviera-baby-blue);
}

/* Page Transitions */
.page-enter-active,
.page-leave-active {
  transition: all 0.3s ease;
}

.page-enter-from {
  opacity: 0;
  transform: translateY(10px);
}

.page-leave-to {
  opacity: 0;
  transform: translateY(-10px);
}
</style>
