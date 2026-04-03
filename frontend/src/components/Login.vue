<template>
  <div class="login-container">
    <div class="login-card">
      <h1>{{ isLogin ? 'Login' : 'Register' }}</h1>

      <form @submit.prevent="handleSubmit">
        <div class="form-group">
          <input
            v-model="username"
            type="text"
            placeholder="Username"
            required
          />
        </div>

        <div class="form-group" v-if="!isLogin">
          <input
            v-model="email"
            type="email"
            placeholder="Email"
            required
          />
        </div>

        <div class="form-group">
          <input
            v-model="password"
            type="password"
            placeholder="Password"
            required
          />
        </div>

        <div class="form-group" v-if="!isLogin">
          <input
            v-model="confirmPassword"
            type="password"
            placeholder="Confirm Password"
            required
          />
        </div>

        <button type="submit" :disabled="loading">
          {{ loading ? 'Loading...' : (isLogin ? 'Login' : 'Register') }}
        </button>

        <p v-if="error" class="error">{{ error }}</p>

        <button type="button" @click="toggleMode" class="toggle-btn">
          {{ isLogin ? 'Need an account? Register' : 'Have an account? Login' }}
        </button>

        <button type="button" @click="loginAsGuest" :disabled="loading" class="guest-btn">
          {{ loading ? 'Loading...' : 'Login as Guest' }}
        </button>
      </form>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { auth, type AuthResponse } from '@/services/auth'

defineOptions({
  name: 'LoginForm'
})

const router = useRouter()
const isLogin = ref(true)
const loading = ref(false)
const error = ref('')

const username = ref('')
const email = ref('')
const password = ref('')
const confirmPassword = ref('')

const handleSubmit = async () => {
  error.value = ''

  // Validation
  if (!isLogin.value && password.value !== confirmPassword.value) {
    error.value = 'Passwords do not match'
    return
  }

  if (!username.value || !password.value) {
    error.value = 'Username and password are required'
    return
  }

  loading.value = true

  try {
    let authResponse: AuthResponse

    if (isLogin.value) {
      // Login
      authResponse = await auth.login({
        username: username.value,
        password: password.value,
      })
    } else {
      // Register
      await auth.register({
        username: username.value,
        email: email.value,
        password: password.value,
      })

      // Auto-login after registration
      authResponse = await auth.login({
        username: username.value,
        password: password.value,
      })
    }

    // Store auth data
    localStorage.setItem('authToken', authResponse.token)
    localStorage.setItem('username', authResponse.username)
    localStorage.setItem('userId', authResponse.userId)

    // Redirect to home/assets page
    router.push('/assets')
  } catch (err) {
    error.value = err instanceof Error ? err.message : 'Authentication failed'
  } finally {
    loading.value = false
  }
}

const toggleMode = () => {
  isLogin.value = !isLogin.value
  error.value = ''
  // Clear form when switching modes
  password.value = ''
  confirmPassword.value = ''
}

const loginAsGuest = async () => {
  loading.value = true
  try {
    auth.loginAsGuest()
    router.push('/assets')
  } catch {
    error.value = 'Guest login failed'
  } finally {
    loading.value = false
  }
}
</script>

<style scoped>
.login-container {
  display: flex;
  justify-content: center;
  align-items: center;
  min-height: 100vh;
  width: 100%;
  background: #f5f5f5;
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
}

.login-card {
  background: white;
  padding: 2rem;
  border-radius: 8px;
  box-shadow: 0 2px 10px rgba(0,0,0,0.1);
  width: 100%;
  max-width: 400px;
}

/* Rest of your styles remain the same */
h1 {
  text-align: center;
  margin-bottom: 1.5rem;
  color: #42b883;
}

.form-group {
  margin-bottom: 1rem;
}

input {
  width: 100%;
  padding: 10px;
  border: 1px solid #ddd;
  border-radius: 4px;
  font-size: 14px;
  box-sizing: border-box;
}

input:focus {
  outline: none;
  border-color: #42b883;
  box-shadow: 0 0 0 2px rgba(66, 184, 131, 0.2);
}

button {
  width: 100%;
  padding: 10px;
  background: #42b883;
  color: white;
  border: none;
  border-radius: 4px;
  cursor: pointer;
  font-size: 16px;
  margin-bottom: 1rem;
}

button:hover:not(:disabled) {
  background: #33a06f;
}

button:disabled {
  background: #ccc;
  cursor: not-allowed;
}

.error {
  color: #dc2626;
  text-align: center;
  margin-bottom: 1rem;
  font-size: 14px;
}

.toggle-btn {
  background: none;
  color: #42b883;
  border: none;
  cursor: pointer;
  font-size: 14px;
  width: 100%;
  padding: 8px;
}

.toggle-btn:hover {
  text-decoration: underline;
}

.guest-btn {
  background: none !important;
  color: #6b7280 !important;
  border: 1px solid #6b7280 !important;
  cursor: pointer;
  font-size: 14px;
  width: 100%;
  padding: 10px;
  margin-bottom: 1rem;
  border-radius: 4px;
  transition: all 0.2s;
}

.guest-btn:hover:not(:disabled) {
  background: #f3f4f6 !important;
  color: #374151 !important;
  border-color: #374151 !important;
}

.guest-btn:disabled {
  background: #f3f4f6 !important;
  color: #9ca3af !important;
  border-color: #d1d5db !important;
  cursor: not-allowed;
}
</style>
