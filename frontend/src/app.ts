/**
 * EduPortal — Frontend Application
 * Week 1: TypeScript, DOM manipulation, localStorage auth
 * Week 2: Connects to ASP.NET Core backend API
 */

import type { Course } from './course.js';
import type { QuizQuestion, QuizResult, CourseQuiz } from './quiz.js';
import type { Student, LoginCredentials, RegisterData } from './student.js';

// ─── API Configuration ───────────────────────────────────────────────────────
const API_BASE = 'http://localhost:5000/api';
let authToken: string | null = localStorage.getItem('edu_token');

async function apiFetch<T>(
  endpoint: string,
  options: RequestInit = {}
): Promise<{ data?: T; error?: string }> {
  try {
    const headers: Record<string, string> = {
      'Content-Type': 'application/json',
    };
    if (authToken) headers['Authorization'] = `Bearer ${authToken}`;

    const res = await fetch(`${API_BASE}${endpoint}`, {
      ...options,
      headers: { ...headers, ...(options.headers as Record<string, string> || {}) },
    });

    if (res.status === 401) {
      logout();
      return { error: 'Session expired. Please log in again.' };
    }

    const json = await res.json().catch(() => ({}));
    if (!res.ok) return { error: json.message || `Error ${res.status}` };
    return { data: json as T };
  } catch {
    return { error: 'Cannot connect to server. Make sure the backend is running.' };
  }
}

// ─── Course Data (fallback when backend is not running) ──────────────────────
const COURSES: Course[] = [
  { id: 'c1', name: 'C# Programming Fundamentals', instructor: 'Dr. Ananya Sharma', description: 'Master the basics of C# — variables, OOP, collections, LINQ, async programming and more.', category: 'Backend', duration: '3 hrs', level: 'Beginner', icon: '💻' },
  { id: 'c2', name: 'Essentials of .NET', instructor: 'Prof. Rajan Mehta', description: 'Deep dive into the .NET ecosystem: EF Core, Design Patterns, Unit Testing, and Swagger.', category: 'Backend', duration: '10 hrs', level: 'Intermediate', icon: '⚙️' },
  { id: 'c3', name: 'ASP.NET Core Web APIs', instructor: 'Ms. Priya Nair', description: 'Build production-grade RESTful APIs with ASP.NET Core, middleware, JWT auth, and TDD.', category: 'Web', duration: '9 hrs', level: 'Intermediate', icon: '🌐' },
  { id: 'c4', name: 'Advanced Backend Concepts', instructor: 'Mr. Arjun Patel', description: 'SignalR, Microservices, Docker, OAuth2, caching strategies, and API versioning.', category: 'Advanced', duration: '15 hrs', level: 'Advanced', icon: '🚀' },
  { id: 'c5', name: 'Entity Framework Core', instructor: 'Dr. Kavitha Rao', description: 'ORM mastery: Code-First, migrations, LINQ queries, relationships and performance tuning.', category: 'Data', duration: '6 hrs', level: 'Intermediate', icon: '🗄️' },
  { id: 'c6', name: 'Docker & Containerization', instructor: 'Mr. Vikram Singh', description: 'Containerize .NET apps with Docker, multi-stage builds, Docker Compose and Kubernetes basics.', category: 'DevOps', duration: '4 hrs', level: 'Intermediate', icon: '🐳' },
];

const QUIZ_DATA: Record<string, QuizQuestion[]> = {
  c1: [
    { question: 'Which keyword is used to declare a variable whose type is inferred at compile time?', options: ['dynamic', 'var', 'object', 'auto'], correctIndex: 1 },
    { question: 'What is the correct way to declare a nullable integer in C#?', options: ['int? x', 'nullable int x', 'int x = null', 'Nullable x'], correctIndex: 0 },
    { question: 'Which collection guarantees unique elements and O(1) lookup?', options: ['List<T>', 'Dictionary<K,V>', 'HashSet<T>', 'Queue<T>'], correctIndex: 2 },
    { question: 'What does the "async" keyword do to a method?', options: ['Makes it run on a new thread', 'Allows use of await inside it', 'Runs it in parallel automatically', 'Disables exception handling'], correctIndex: 1 },
    { question: 'Which type should you use for storing currency/financial values?', options: ['double', 'float', 'decimal', 'long'], correctIndex: 2 },
  ],
  c2: [
    { question: 'What does EF Core\'s .AsNoTracking() do?', options: ['Disables LINQ', 'Improves read performance by not tracking entities', 'Deletes entities after read', 'Caches query results'], correctIndex: 1 },
    { question: 'Which DI lifetime creates a new instance for every HTTP request?', options: ['Transient', 'Scoped', 'Singleton', 'Pooled'], correctIndex: 1 },
    { question: 'What does the Repository pattern achieve?', options: ['Faster SQL queries', 'Abstraction of data access logic', 'Automatic caching', 'Dependency injection'], correctIndex: 1 },
    { question: 'In TDD, what is the correct order?', options: ['Green → Red → Refactor', 'Red → Green → Refactor', 'Refactor → Red → Green', 'Write code → Test → Refactor'], correctIndex: 1 },
    { question: 'What is the purpose of IQueryable vs IEnumerable in EF Core?', options: ['They are identical', 'IQueryable executes SQL in database; IEnumerable in memory', 'IEnumerable is faster', 'IQueryable only works with XML'], correctIndex: 1 },
  ],
  c3: [
    { question: 'What HTTP status code means "resource created successfully"?', options: ['200', '201', '204', '202'], correctIndex: 1 },
    { question: 'What does [ApiController] attribute do in ASP.NET Core?', options: ['Enables Swagger', 'Enables automatic model validation and binding', 'Adds JWT auth', 'Enables CORS'], correctIndex: 1 },
    { question: 'Which middleware must come BEFORE UseAuthorization?', options: ['UseRouting', 'UseAuthentication', 'UseStaticFiles', 'UseHttpsRedirection'], correctIndex: 1 },
    { question: 'What does CORS stand for?', options: ['Cross-Origin Resource Sharing', 'Client Object Request System', 'Common Object Resource Service', 'Cross-Object Routing Service'], correctIndex: 0 },
    { question: 'Which HTTP method is idempotent and used for full updates?', options: ['POST', 'PATCH', 'PUT', 'GET'], correctIndex: 2 },
  ],
  c4: [
    { question: 'What is SignalR used for?', options: ['Database migrations', 'Real-time bidirectional communication', 'JWT generation', 'File uploads'], correctIndex: 1 },
    { question: 'In microservices, what is the API Gateway responsible for?', options: ['Direct database access', 'Single entry point for all client requests', 'Running background jobs', 'Storing shared state'], correctIndex: 1 },
    { question: 'Which JWT claim stores user roles by convention?', options: ['sub', 'iss', 'role', 'aud'], correctIndex: 2 },
    { question: 'What Docker command builds and starts all services?', options: ['docker run all', 'docker-compose up', 'docker start', 'docker build --all'], correctIndex: 1 },
    { question: 'Which caching strategy checks cache first, loads from DB on miss?', options: ['Write-Through', 'Write-Behind', 'Cache-Aside', 'Read-Through Push'], correctIndex: 2 },
  ],
  c5: [
    { question: 'What does Code-First approach mean in EF Core?', options: ['Write SQL first, then generate classes', 'Write C# classes first, generate DB schema', 'Use stored procedures', 'Use XML config files'], correctIndex: 1 },
    { question: 'What command applies pending EF Core migrations?', options: ['dotnet ef apply', 'dotnet ef database update', 'dotnet migrate', 'dotnet ef push'], correctIndex: 1 },
    { question: 'What is the N+1 query problem?', options: ['Loading N records with 1 query', '1 query for list + N queries for related data', 'Running queries in parallel', 'Using N indexes'], correctIndex: 1 },
    { question: 'Which method loads related entities in a single SQL JOIN?', options: ['.ThenInclude()', '.Include()', '.Select()', '.Join()'], correctIndex: 1 },
    { question: 'What does .AsNoTracking() affect?', options: ['Insert operations', 'Read performance — skips change tracking', 'Delete operations', 'Migration scripts'], correctIndex: 1 },
  ],
  c6: [
    { question: 'What is the purpose of multi-stage Docker builds?', options: ['Faster internet downloads', 'Smaller final image by separating build and runtime', 'Better security', 'Multiple entry points'], correctIndex: 1 },
    { question: 'What does docker-compose.yml define?', options: ['CI/CD pipeline', 'Multi-container application services', 'Kubernetes pods', 'Network firewall rules'], correctIndex: 1 },
    { question: 'Which .NET base image should be used for the runtime (final) stage?', options: ['mcr.microsoft.com/dotnet/sdk', 'mcr.microsoft.com/dotnet/aspnet', 'ubuntu:latest', 'alpine:latest'], correctIndex: 1 },
    { question: 'What does the EXPOSE instruction in Dockerfile do?', options: ['Opens the port on the host', 'Documents the port the container listens on', 'Automatically maps port to host', 'Starts the web server'], correctIndex: 1 },
    { question: 'What is the purpose of .dockerignore?', options: ['Ignores Docker commands', 'Excludes files from Docker build context', 'Disables Docker networking', 'Hides environment variables'], correctIndex: 1 },
  ],
};

// ─── State ────────────────────────────────────────────────────────────────────
let currentStudent: Student | null = null;
let useBackend = false; // Will be set to true if backend responds

// ─── DOM Helpers ──────────────────────────────────────────────────────────────
const $  = <T extends HTMLElement>(id: string) => document.getElementById(id) as T;
const $$ = (sel: string) => document.querySelectorAll(sel);

function showToast(message: string, type: 'success' | 'error' = 'success'): void {
  const t = document.createElement('div');
  t.className = `toast toast-${type}`;
  t.textContent = message;
  document.body.appendChild(t);
  requestAnimationFrame(() => t.classList.add('show'));
  setTimeout(() => {
    t.classList.remove('show');
    setTimeout(() => t.remove(), 400);
  }, 3200);
}

function setError(id: string, msg: string): void {
  const el = $(id);
  if (el) el.textContent = msg;
}

// ─── Auth ─────────────────────────────────────────────────────────────────────
async function checkBackend(): Promise<void> {
  const result = await apiFetch<{ message: string }>('/health');
  useBackend = !result.error;
}

function simpleHash(s: string): string {
  let h = 0;
  for (let i = 0; i < s.length; i++) h = Math.imul(31, h) + s.charCodeAt(i) | 0;
  return h.toString(16);
}

function getLocalStudents(): Student[] {
  return JSON.parse(localStorage.getItem('edu_students') || '[]');
}

function saveLocalStudents(students: Student[]): void {
  localStorage.setItem('edu_students', JSON.stringify(students));
}

async function handleLogin(e: Event): Promise<void> {
  e.preventDefault();
  setError('login-error', '');
  const email    = ($<HTMLInputElement>('login-email')).value.trim();
  const password = ($<HTMLInputElement>('login-password')).value;

  if (!email || !password) { setError('login-error', 'Please fill in all fields.'); return; }

  if (useBackend) {
    const result = await apiFetch<{ token: string; student: Student }>('/auth/login', {
      method: 'POST',
      body: JSON.stringify({ email, password } as LoginCredentials),
    });
    if (result.error) { setError('login-error', result.error); return; }
    authToken = result.data!.token;
    localStorage.setItem('edu_token', authToken);
    currentStudent = result.data!.student;
  } else {
    // Local fallback
    const students = getLocalStudents();
    const student = students.find(s => s.email === email && s.passwordHash === simpleHash(password));
    if (!student) { setError('login-error', 'Invalid email or password.'); return; }
    currentStudent = student;
  }

  localStorage.setItem('edu_current', JSON.stringify(currentStudent));
  initApp();
}

async function handleRegister(e: Event): Promise<void> {
  e.preventDefault();
  setError('register-error', '');
  const name     = ($<HTMLInputElement>('reg-name')).value.trim();
  const email    = ($<HTMLInputElement>('reg-email')).value.trim();
  const password = ($<HTMLInputElement>('reg-password')).value;

  if (!name || !email || !password) { setError('register-error', 'Please fill in all fields.'); return; }
  if (password.length < 6) { setError('register-error', 'Password must be at least 6 characters.'); return; }
  if (!email.includes('@')) { setError('register-error', 'Please enter a valid email address.'); return; }

  if (useBackend) {
    const result = await apiFetch<{ token: string; student: Student }>('/auth/register', {
      method: 'POST',
      body: JSON.stringify({ name, email, password } as RegisterData),
    });
    if (result.error) { setError('register-error', result.error); return; }
    authToken = result.data!.token;
    localStorage.setItem('edu_token', authToken);
    currentStudent = result.data!.student;
  } else {
    const students = getLocalStudents();
    if (students.find(s => s.email === email)) { setError('register-error', 'Email already registered.'); return; }
    const newStudent: Student = {
      id: crypto.randomUUID(),
      name, email,
      passwordHash: simpleHash(password),
      enrolledCourseIds: [],
      joinedDate: new Date().toLocaleDateString('en-IN', { day: 'numeric', month: 'long', year: 'numeric' }),
    };
    students.push(newStudent);
    saveLocalStudents(students);
    currentStudent = newStudent;
  }

  localStorage.setItem('edu_current', JSON.stringify(currentStudent));
  initApp();
}

function logout(): void {
  currentStudent = null;
  authToken = null;
  localStorage.removeItem('edu_token');
  localStorage.removeItem('edu_current');
  $('section-auth').style.display = 'flex';
  $('main-nav').style.display = 'none';
  ($('.app-content') as HTMLElement).style.display = 'none';
  ($<HTMLInputElement>('login-email')).value = '';
  ($<HTMLInputElement>('login-password')).value = '';
}

// ─── Navigation ───────────────────────────────────────────────────────────────
function switchTab(tab: string): void {
  $$('.nav-tab').forEach(t => t.classList.remove('active'));
  $$('.section').forEach(s => s.classList.remove('active'));
  document.querySelector(`[data-tab="${tab}"]`)?.classList.add('active');
  $(`section-${tab.replace('-', '-')}`)?.classList.add('active');
}

// ─── Courses ──────────────────────────────────────────────────────────────────
async function loadCourses(): Promise<Course[]> {
  if (useBackend) {
    const result = await apiFetch<Course[]>('/courses');
    if (result.data) return result.data;
  }
  return COURSES;
}

async function renderCourses(): Promise<void> {
  const grid = $('courses-grid');
  grid.innerHTML = '<div class="empty-state"><div class="spinner"></div><p>Loading courses…</p></div>';
  const courses = await loadCourses();
  const enrolled = currentStudent?.enrolledCourseIds ?? [];

  grid.innerHTML = courses.map(c => `
    <div class="course-card" data-id="${c.id}">
      <span class="course-badge level-${c.level.toLowerCase()}">${c.level}</span>
      <div class="course-icon">${c.icon}</div>
      <div class="course-title">${c.name}</div>
      <div class="course-instructor">👨‍🏫 ${c.instructor}</div>
      <div class="course-desc">${c.description}</div>
      <div class="course-meta">
        <span class="meta-tag">📁 ${c.category}</span>
        <span class="meta-tag">⏱ ${c.duration}</span>
      </div>
      <button class="btn-enroll ${enrolled.includes(c.id) ? 'enrolled' : ''}"
              data-cid="${c.id}" ${enrolled.includes(c.id) ? 'disabled' : ''}>
        ${enrolled.includes(c.id) ? '✅ Enrolled' : '+ Enroll Now'}
      </button>
    </div>`).join('');

  grid.querySelectorAll('.btn-enroll:not(.enrolled)').forEach(btn => {
    btn.addEventListener('click', () => enrollCourse((btn as HTMLElement).dataset.cid!));
  });
}

async function enrollCourse(courseId: string): Promise<void> {
  if (!currentStudent) return;

  if (useBackend) {
    const result = await apiFetch(`/courses/${courseId}/enroll`, { method: 'POST' });
    if (result.error) { showToast(result.error, 'error'); return; }
  } else {
    if (currentStudent.enrolledCourseIds.includes(courseId)) return;
    currentStudent.enrolledCourseIds.push(courseId);
    const students = getLocalStudents().map(s =>
      s.id === currentStudent!.id ? currentStudent! : s
    );
    saveLocalStudents(students);
    localStorage.setItem('edu_current', JSON.stringify(currentStudent));
  }

  const course = COURSES.find(c => c.id === courseId);
  showToast(`Enrolled in "${course?.name}"!`, 'success');
  await renderCourses();
  renderMyCourses();
  renderProfileCourses();
}

// ─── My Courses ───────────────────────────────────────────────────────────────
function getQuizResults(): Record<string, QuizResult> {
  return JSON.parse(localStorage.getItem('edu_quiz_results') || '{}');
}

function renderMyCourses(): void {
  const list = $('enrolled-list');
  const enrolled = currentStudent?.enrolledCourseIds ?? [];
  if (!enrolled.length) {
    list.innerHTML = `<div class="empty-state"><div class="empty-icon">📚</div><h3>No courses yet</h3><p>Go to <strong>Courses</strong> and enroll in something!</p></div>`;
    return;
  }

  const results = getQuizResults();
  list.innerHTML = COURSES.filter(c => enrolled.includes(c.id)).map(c => {
    const r = results[c.id];
    const scoreHtml = r
      ? `<span class="quiz-score ${r.passed ? 'pass' : 'fail'}">
           ${r.passed ? '✅' : '❌'} ${r.score}/${r.total} (${r.percentage}%) — ${r.date}
         </span>`
      : `<span class="quiz-score pending">📝 Quiz Pending</span>`;
    return `
      <div class="enrolled-card">
        <div class="enrolled-icon">${c.icon}</div>
        <div class="enrolled-info">
          <h3>${c.name}</h3>
          <p>👨‍🏫 ${c.instructor} &nbsp;·&nbsp; ⏱ ${c.duration} &nbsp;·&nbsp; 📁 ${c.category}</p>
          ${scoreHtml}
        </div>
        <button class="btn-quiz" data-cid="${c.id}" data-cname="${c.name}">
          ${r ? '🔄 Retake Quiz' : '📝 Take Quiz'}
        </button>
      </div>`;
  }).join('');

  list.querySelectorAll('.btn-quiz').forEach(btn => {
    btn.addEventListener('click', () => {
      const cid = (btn as HTMLElement).dataset.cid!;
      const cname = (btn as HTMLElement).dataset.cname!;
      startQuiz(cid, cname);
      switchTab('quiz');
    });
  });
}

// ─── Quiz ─────────────────────────────────────────────────────────────────────
function startQuiz(courseId: string, courseName: string): void {
  const questions = QUIZ_DATA[courseId];
  if (!questions) return;
  const container = $('quiz-container');

  container.innerHTML = `
    <div class="quiz-header">
      <h2>📝 ${courseName}</h2>
      <p class="quiz-sub">Answer all ${questions.length} questions, then submit.</p>
    </div>
    <div class="quiz-questions">
      ${questions.map((q, qi) => `
        <div class="quiz-question" data-qi="${qi}">
          <div class="q-text"><span class="q-num">Q${qi + 1}.</span> ${q.question}</div>
          <div class="q-options">
            ${q.options.map((opt, oi) => `
              <label class="q-option">
                <input type="radio" name="q${qi}" value="${oi}" />
                <span class="option-letter">${String.fromCharCode(65 + oi)}</span>
                ${opt}
              </label>`).join('')}
          </div>
        </div>`).join('')}
    </div>
    <button id="submit-quiz-btn" class="btn-primary">Submit Quiz →</button>`;

  $('submit-quiz-btn').addEventListener('click', () => submitQuiz(courseId, courseName, questions));
}

function submitQuiz(courseId: string, courseName: string, questions: QuizQuestion[]): void {
  let score = 0;
  const unanswered: number[] = [];

  questions.forEach((q, qi) => {
    const selected = document.querySelector<HTMLInputElement>(`input[name="q${qi}"]:checked`);
    if (!selected) { unanswered.push(qi + 1); return; }
    if (parseInt(selected.value) === q.correctIndex) score++;
  });

  if (unanswered.length) {
    showToast(`Please answer question(s): ${unanswered.join(', ')}`, 'error');
    return;
  }

  const total = questions.length;
  const percentage = Math.round((score / total) * 100);
  const passed = percentage >= 60;

  const result: QuizResult = {
    courseId, courseName, score, total, percentage,
    date: new Date().toLocaleDateString('en-IN'),
    passed,
  };

  if (useBackend) {
    apiFetch(`/quiz/${courseId}/submit`, {
      method: 'POST',
      body: JSON.stringify(result),
    });
  }

  const stored = getQuizResults();
  stored[courseId] = result;
  localStorage.setItem('edu_quiz_results', JSON.stringify(stored));

  const container = $('quiz-container');
  container.innerHTML = `
    <div class="quiz-result ${passed ? 'pass' : 'fail'}">
      <div class="result-emoji">${passed ? '🎉' : '😔'}</div>
      <h2>${passed ? 'Congratulations!' : 'Keep Practicing!'}</h2>
      <p class="result-course">${courseName}</p>
      <div class="result-score-ring">
        <span class="score-num" style="color:${passed ? 'var(--green)' : 'var(--red)'}">${percentage}%</span>
        <span class="score-label">Score</span>
      </div>
      <div class="result-detail">
        <span>✅ Correct: <strong>${score}</strong></span>
        <span>❌ Wrong: <strong>${total - score}</strong></span>
        <span>📋 Total: <strong>${total}</strong></span>
      </div>
      <p class="result-status ${passed ? 'pass' : 'fail'}">${passed ? '🏆 PASSED (≥ 60%)' : '📚 FAILED (< 60%)'}</p>
      <button id="back-to-courses-btn" class="btn-primary">← Back to My Courses</button>
    </div>`;

  $('back-to-courses-btn').addEventListener('click', () => {
    switchTab('my-courses');
    renderMyCourses();
  });

  showToast(passed ? `You passed with ${percentage}%! 🎉` : `Score: ${percentage}%. Try again!`, passed ? 'success' : 'error');
}

// ─── Weather ──────────────────────────────────────────────────────────────────
async function fetchWeather(city: string): Promise<void> {
  const container = $('weather-container');
  container.innerHTML = `<div class="weather-loading"><div class="spinner"></div><p>Fetching weather for ${city}…</p></div>`;

  // Use Open-Meteo (free, no API key) + geocoding
  try {
    const geoRes = await fetch(`https://geocoding-api.open-meteo.com/v1/search?name=${encodeURIComponent(city)}&count=1&language=en&format=json`);
    const geoData = await geoRes.json();
    if (!geoData.results?.length) throw new Error('City not found');

    const { latitude, longitude, name, country } = geoData.results[0];
    const wxRes = await fetch(
      `https://api.open-meteo.com/v1/forecast?latitude=${latitude}&longitude=${longitude}&current=temperature_2m,relative_humidity_2m,wind_speed_10m,weathercode,apparent_temperature&timezone=auto`
    );
    const wxData = await wxRes.json();
    const c = wxData.current;

    const codeMap: Record<number, [string, string]> = {
      0: ['☀️', 'Clear Sky'], 1: ['🌤', 'Mainly Clear'], 2: ['⛅', 'Partly Cloudy'], 3: ['☁️', 'Overcast'],
      45: ['🌫', 'Foggy'], 48: ['🌫', 'Rime Fog'],
      51: ['🌦', 'Light Drizzle'], 53: ['🌦', 'Drizzle'], 55: ['🌧', 'Heavy Drizzle'],
      61: ['🌧', 'Slight Rain'], 63: ['🌧', 'Moderate Rain'], 65: ['🌧', 'Heavy Rain'],
      71: ['🌨', 'Slight Snow'], 73: ['❄️', 'Moderate Snow'], 75: ['❄️', 'Heavy Snow'],
      80: ['🌦', 'Rain Showers'], 81: ['🌧', 'Showers'], 82: ['⛈', 'Heavy Showers'],
      95: ['⛈', 'Thunderstorm'], 96: ['⛈', 'Thunderstorm+Hail'], 99: ['⛈', 'Heavy Thunderstorm'],
    };
    const [icon, condition] = codeMap[c.weathercode] ?? ['🌡', 'Unknown'];

    container.innerHTML = `
      <div class="weather-card">
        <div class="weather-location">
          <h2>📍 ${name}, ${country}</h2>
          <div class="weather-date">${new Date().toLocaleDateString('en-IN', { weekday: 'long', day: 'numeric', month: 'long', year: 'numeric' })}</div>
        </div>
        <div class="weather-main">
          <div style="font-size:5rem;line-height:1">${icon}</div>
          <div class="weather-temp">${Math.round(c.temperature_2m)}°C</div>
          <div class="weather-condition">${condition}</div>
          <div class="weather-desc">Feels like ${Math.round(c.apparent_temperature)}°C</div>
        </div>
        <div class="weather-stats">
          <div class="weather-stat"><span class="stat-icon">💧</span><span class="stat-val">${c.relative_humidity_2m}%</span><span class="stat-label">Humidity</span></div>
          <div class="weather-stat"><span class="stat-icon">💨</span><span class="stat-val">${c.wind_speed_10m} km/h</span><span class="stat-label">Wind</span></div>
          <div class="weather-stat"><span class="stat-icon">🌡</span><span class="stat-val">${Math.round(c.apparent_temperature)}°C</span><span class="stat-label">Feels Like</span></div>
        </div>
      </div>`;
  } catch (err) {
    const msg = err instanceof Error ? err.message : 'Unknown error';
    container.innerHTML = `
      <div class="weather-error">
        <div class="weather-error-icon">🌧</div>
        <h3>Couldn't fetch weather</h3>
        <p>${msg}</p>
      </div>`;
  }
}

// ─── Profile ──────────────────────────────────────────────────────────────────
function renderProfile(): void {
  if (!currentStudent) return;
  $('profile-name').textContent   = currentStudent.name;
  $('profile-email').textContent  = currentStudent.email;
  $('profile-joined').textContent = currentStudent.joinedDate;
  $('profile-course-count').textContent = String(currentStudent.enrolledCourseIds.length);
  ($<HTMLInputElement>('edit-name')).value = currentStudent.name;
  renderProfileCourses();
}

function renderProfileCourses(): void {
  const list = $('profile-courses-list');
  const enrolled = currentStudent?.enrolledCourseIds ?? [];
  if (!enrolled.length) {
    list.innerHTML = '<p class="empty-notice">No courses enrolled yet.</p>';
    return;
  }
  list.innerHTML = COURSES.filter(c => enrolled.includes(c.id))
    .map(c => `<div class="profile-course-chip">${c.icon} ${c.name}</div>`).join('');
}

async function saveProfile(): Promise<void> {
  if (!currentStudent) return;
  const newName = ($<HTMLInputElement>('edit-name')).value.trim();
  const msg = $('profile-msg');

  if (!newName) { msg.textContent = 'Name cannot be empty.'; msg.className = 'profile-msg error'; return; }

  if (useBackend) {
    const result = await apiFetch('/students/profile', {
      method: 'PUT',
      body: JSON.stringify({ name: newName }),
    });
    if (result.error) { msg.textContent = result.error; msg.className = 'profile-msg error'; return; }
  }

  currentStudent.name = newName;
  const students = getLocalStudents().map(s => s.id === currentStudent!.id ? currentStudent! : s);
  saveLocalStudents(students);
  localStorage.setItem('edu_current', JSON.stringify(currentStudent));

  $('profile-name').textContent = newName;
  $('welcome-name').textContent = newName.split(' ')[0];
  msg.textContent = '✅ Profile updated successfully!';
  msg.className = 'profile-msg success';
  setTimeout(() => { msg.textContent = ''; }, 3000);
}

// ─── App Init ─────────────────────────────────────────────────────────────────
function initApp(): void {
  if (!currentStudent) return;

  $('section-auth').style.display = 'none';
  $('main-nav').style.display = 'flex';
  ($('.app-content') as HTMLElement).style.display = 'block';
  $('welcome-name').textContent = currentStudent.name.split(' ')[0];

  renderCourses();
  renderMyCourses();
  renderProfile();
  fetchWeather('Bangalore');
}

// ─── Boot ─────────────────────────────────────────────────────────────────────
(async function boot(): Promise<void> {
  await checkBackend();

  const saved = localStorage.getItem('edu_current');
  if (saved) {
    currentStudent = JSON.parse(saved);
    initApp();
  }

  // Auth form handlers
  $('login-form').addEventListener('submit', handleLogin);
  $('register-form').addEventListener('submit', handleRegister);
  $('show-register').addEventListener('click', () => {
    $('login-panel').style.display = 'none';
    $('register-panel').style.display = 'block';
  });
  $('show-login').addEventListener('click', () => {
    $('register-panel').style.display = 'none';
    $('login-panel').style.display = 'block';
  });

  // Nav
  $$('.nav-tab').forEach(tab => {
    tab.addEventListener('click', () => {
      const t = (tab as HTMLElement).dataset.tab!;
      switchTab(t);
      if (t === 'my-courses') renderMyCourses();
      if (t === 'profile') renderProfile();
      if (t === 'weather') fetchWeather(($<HTMLInputElement>('weather-city')).value || 'Bangalore');
    });
  });

  $('logout-btn').addEventListener('click', logout);
  $('weather-search-btn').addEventListener('click', () =>
    fetchWeather(($<HTMLInputElement>('weather-city')).value.trim() || 'Bangalore'));
  $<HTMLInputElement>('weather-city').addEventListener('keydown', (e) => {
    if (e.key === 'Enter') fetchWeather(($<HTMLInputElement>('weather-city')).value.trim() || 'Bangalore');
  });
  $('save-profile-btn').addEventListener('click', saveProfile);
})();
