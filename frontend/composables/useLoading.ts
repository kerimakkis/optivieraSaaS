// Global loading state
const globalLoadingState = ref(false)
const globalLoadingText = ref('Loading...')

export const useLoading = () => {
  const start = (text?: string) => {
    globalLoadingText.value = text || 'Loading...'
    globalLoadingState.value = true
  }

  const finish = () => {
    globalLoadingState.value = false
  }

  const setText = (text: string) => {
    globalLoadingText.value = text
  }

  return {
    isLoading: readonly(globalLoadingState),
    loadingText: readonly(globalLoadingText),
    start,
    finish,
    setText
  }
}
