export const useApiLoading = () => {
  const { start, finish, setText } = useLoading()

  const withLoading = async <T>(
    apiCall: () => Promise<T>,
    loadingText: string = 'Loading...'
  ): Promise<T> => {
    try {
      start(loadingText)
      const result = await apiCall()
      return result
    } finally {
      finish()
    }
  }

  const withAsyncLoading = async <T>(
    apiCall: () => Promise<T>,
    loadingText: string = 'Loading...'
  ): Promise<T> => {
    start(loadingText)
    
    try {
      const result = await apiCall()
      return result
    } catch (error) {
      throw error
    } finally {
      // Don't auto-finish, let the caller handle it
    }
  }

  return {
    withLoading,
    withAsyncLoading,
    start,
    finish,
    setText
  }
}
