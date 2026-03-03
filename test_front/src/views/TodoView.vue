<script setup>
import { ref, onMounted } from 'vue'

const todos = ref([])
const newTitle = ref('')
const editingId = ref(null)
const editingTitle = ref('')
const loading = ref(false)
const errorMsg = ref('')

async function fetchTodos() {
  loading.value = true
  errorMsg.value = ''
  try {
    const res = await fetch('http://localhost:5030/todos')
    if (!res.ok) throw new Error(`서버 오류: ${res.status}`)
    todos.value = await res.json()
  } catch (e) {
    errorMsg.value = e.message
    console.error(e)
  } finally {
    loading.value = false
  }
}

async function addTodo() {
  const title = newTitle.value.trim()
  if (!title) return
  try {
    const res = await fetch('http://localhost:5030/todos', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ title, isCompleted: false }),
    })
    if (!res.ok) throw new Error(`추가 실패: ${res.status}`)
    newTitle.value = ''
    await fetchTodos()
  } catch (e) {
    errorMsg.value = e.message
    console.error(e)
  }
}

async function toggleComplete(todo) {
  const prev = todo.isCompleted
  // 즉시 UI 반영 (낙관적 업데이트)
  todo.isCompleted = !prev
  try {
    const res = await fetch(`http://localhost:5030/todos/${todo.id}`, {
      method: 'PUT',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ ...todo, isCompleted: todo.isCompleted }),
    })
    if (!res.ok) throw new Error(`수정 실패: ${res.status}`)
  } catch (e) {
    // 실패 시 롤백
    todo.isCompleted = prev
    errorMsg.value = e.message
    console.error(e)
  }
}

function startEdit(todo) {
  editingId.value = todo.id
  editingTitle.value = todo.title
}

function cancelEdit() {
  editingId.value = null
  editingTitle.value = ''
}

async function saveEdit(todo) {
  const title = editingTitle.value.trim()
  if (!title) return
  try {
    const res = await fetch(`http://localhost:5030/todos/${todo.id}`, {
      method: 'PUT',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ ...todo, title }),
    })
    if (!res.ok) throw new Error(`수정 실패: ${res.status}`)
    cancelEdit()
    await fetchTodos()
  } catch (e) {
    errorMsg.value = e.message
    console.error(e)
  }
}

async function deleteTodo(id) {
  if (!confirm('삭제하시겠습니까?')) return
  try {
    const res = await fetch(`http://localhost:5030/todos/${id}`, { method: 'DELETE' })
    if (!res.ok) throw new Error(`삭제 실패: ${res.status}`)
    await fetchTodos()
  } catch (e) {
    errorMsg.value = e.message
    console.error(e)
  }
}

onMounted(fetchTodos)
</script>

<template>
  <div class="todo-container">
    <h1>Todo 목록</h1>

    <!-- 에러 메시지 -->
    <p v-if="errorMsg" class="error">{{ errorMsg }}</p>

    <!-- 추가 폼 -->
    <div class="add-form">
      <input v-model="newTitle" placeholder="새 할 일 입력..." @keyup.enter="addTodo" />
      <button @click="addTodo">추가</button>
    </div>

    <!-- 로딩 -->
    <p v-if="loading">불러오는 중...</p>

    <!-- 목록 없음 -->
    <p v-else-if="todos.length === 0">할 일이 없습니다.</p>

    <!-- 목록 -->
    <ul v-else>
      <li v-for="todo in todos" :key="todo.id" :class="{ done: todo.isCompleted }">
        <!-- 수정 모드 -->
        <template v-if="editingId === todo.id">
          <input
            v-model="editingTitle"
            class="edit-input"
            @keyup.enter="saveEdit(todo)"
            @keyup.esc="cancelEdit"
          />
          <div class="actions">
            <button class="save" @click="saveEdit(todo)">저장</button>
            <button class="cancel" @click="cancelEdit">취소</button>
          </div>
        </template>

        <!-- 일반 모드 -->
        <template v-else>
          <label class="todo-label">
            <input type="checkbox" :checked="todo.isCompleted" @change="toggleComplete(todo)" />
            <span>{{ todo.title }}</span>
          </label>
          <div class="actions">
            <button class="edit" @click="startEdit(todo)">수정</button>
            <button class="delete" @click="deleteTodo(todo.id)">삭제</button>
          </div>
        </template>
      </li>
    </ul>
  </div>
</template>

<style scoped>
.todo-container {
  max-width: 600px;
  margin: 40px auto;
  padding: 0 20px;
  font-family: sans-serif;
}

h1 {
  margin-bottom: 20px;
}

.error {
  color: #e74c3c;
  margin-bottom: 12px;
}

.add-form {
  display: flex;
  gap: 8px;
  margin-bottom: 20px;
}

.add-form input {
  flex: 1;
  padding: 8px 12px;
  border: 1px solid #ccc;
  border-radius: 4px;
  font-size: 14px;
}

.add-form button {
  padding: 8px 16px;
  background-color: #4a90d9;
  color: white;
  border: none;
  border-radius: 4px;
  cursor: pointer;
  font-size: 14px;
}

.add-form button:hover {
  background-color: #357ab8;
}

ul {
  list-style: none;
  padding: 0;
  margin: 0;
}

li {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 10px 14px;
  border: 1px solid #ddd;
  border-radius: 6px;
  margin-bottom: 8px;
  background: #fff;
  gap: 8px;
}

li.done span {
  text-decoration: line-through;
  color: #aaa;
}

.todo-label {
  display: flex;
  align-items: center;
  gap: 10px;
  flex: 1;
  cursor: pointer;
}

.edit-input {
  flex: 1;
  padding: 6px 10px;
  border: 1px solid #4a90d9;
  border-radius: 4px;
  font-size: 14px;
}

.actions {
  display: flex;
  gap: 6px;
}

.actions button {
  padding: 5px 10px;
  border: none;
  border-radius: 4px;
  cursor: pointer;
  font-size: 13px;
}

.edit {
  background-color: #f0a500;
  color: white;
}

.edit:hover {
  background-color: #d4900a;
}

.save {
  background-color: #27ae60;
  color: white;
}

.save:hover {
  background-color: #1e8449;
}

.cancel {
  background-color: #95a5a6;
  color: white;
}

.cancel:hover {
  background-color: #7f8c8d;
}

.delete {
  background-color: #e74c3c;
  color: white;
}

.delete:hover {
  background-color: #c0392b;
}
</style>
