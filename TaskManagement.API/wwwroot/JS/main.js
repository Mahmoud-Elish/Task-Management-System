document.addEventListener('DOMContentLoaded', () => {
    const token = localStorage.getItem('authToken');
    if (!token) {
        return;
    }
    fetch('https://localhost:44337/api/Task', {
        headers: {
            'Authorization': `Bearer ${token}`
        }
    }).then(response => {
            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }
            return response.json();
    }).then(data => {
            
            populateTasks(data);
    }).catch(error => console.error('Error:', error));
});
function populateTasks(data) {
    const welcomeElement = document.querySelector('.project-info h1');
    if (welcomeElement) {
        welcomeElement.textContent = `Welcome back ${data.userName}`;
    }
    populateColumn('todoTasks', data.toDo.tasks, '#todoCount');
    populateColumn('inProgressTasks', data.inProgress.tasks, '#inProgressCount');
    populateColumn('completedTasks', data.completed.tasks, '#completedCount');
}
function populateColumn(columnId, tasks, countSelector) {
    const column = document.getElementById(columnId);
    const countElement = document.querySelector(countSelector);
    if (!column) {
        return;
    }
    if (!countElement) {
        return;
    }
    countElement.textContent = tasks.length;
    column.innerHTML = '';
    tasks.forEach(task => {
        const taskElement = createTaskElement(task);
        column.appendChild(taskElement);
    });
}
function createTaskElement(task) {
    const taskElement = document.createElement('div');
    taskElement.className = 'task';
    taskElement.draggable = true;
    taskElement.innerHTML = `
        <div class='task__tags'>
            <span class='task__tag task__tag--${task.priority.toLowerCase()}'>${task.priority}</span>
            <button class='task__options'><i class="fas fa-ellipsis-h"></i></button>
        </div>
        <p>${task.title}</p>
        <div class='task__stats'>
            <span><time datetime="${task.dueDate}"><i class="fas fa-flag"></i> ${new Date(task.dueDate).toLocaleDateString()}</time></span>
            <span><i class="fas fa-comment"></i>3</span>
            <span><i class="fas fa-paperclip"></i>7</span>
        </div>`;
    taskElement.querySelector('.task__options').addEventListener('click', () => openModalForEdit(task));
    return taskElement;
}
function openModalForEdit(task) {
}
//==================================================
document.querySelector('.btn-outline-primary').addEventListener('click', () => {
    openModalForAdd();
});
function openModalForAdd() {
    resetModal();
    document.getElementById('taskModalLabel').textContent = 'Add Task';
    $('#taskModal').modal('show');
}
function resetModal() {
    document.getElementById('taskForm').reset();
    document.getElementById('taskId').value = '';
}
//========================================================
function openModalForEdit(task) {
    document.getElementById('taskId').value = task.id;
    document.getElementById('taskTitle').value = task.title;
    document.getElementById('taskDescription').value = task.description;
    document.getElementById('taskDueDate').value = task.dueDate.split('T')[0];
    document.getElementById('taskPriority').value = convertPriorityToInt(task.priority);
    document.getElementById('taskStatus').value = convertStatusToInt(task.status);

    document.getElementById('taskModalLabel').textContent = 'Edit Task';
    $('#taskModal').modal('show');
}
function convertPriorityToInt(priority) {
    const priorityMap = { 'Low': 0, 'Medium': 1, 'High': 2 };
    return priorityMap[priority] || 0;
}
function convertStatusToInt(status) {
    const statusMap = { 'ToDo': 0, 'InProgress': 1, 'Completed': 2 };
    return statusMap[status] || 0;
}
//================================================
document.addEventListener('DOMContentLoaded', function () {
    const taskForm = document.getElementById('taskForm');
    const saveTaskBtn = document.getElementById('saveTaskBtn');
    const taskModal = document.getElementById('taskModal');

    saveTaskBtn.addEventListener('click', handleFormSubmission);
    taskForm.addEventListener('submit', handleFormSubmission);

    function handleFormSubmission(event) {
        event.preventDefault();
        if (validateForm()) {
            const taskData = getFormData();
            const taskId = document.getElementById('taskId').value;

            if (taskId) {
                updateTask(taskId, taskData);
            } else {
                addTask(taskData);
            }

            $('#taskModal').modal('hide');
        }
    }
    function validateForm() {
        let isValid = true;
        taskForm.classList.add('was-validated');
       
        taskForm.querySelectorAll('[required]').forEach(field => {
            if (!field.value) {
                isValid = false;
                field.classList.add('is-invalid');
            } else {
                field.classList.remove('is-invalid');
            }
        });
        const titleField = document.getElementById('taskTitle');
        if (titleField.value.length < 3 || titleField.value.length > 100) {
            isValid = false;
            titleField.classList.add('is-invalid');
        }
        const dueDateField = document.getElementById('taskDueDate');
        if (new Date(dueDateField.value) < new Date().setHours(0, 0, 0, 0)) {
            isValid = false;
            dueDateField.classList.add('is-invalid');
        }

        return isValid;
    }
    function getFormData() {
        return {
            title: document.getElementById('taskTitle').value,
            description: document.getElementById('taskDescription').value,
            dueDate: document.getElementById('taskDueDate').value,
            priority: parseInt(document.getElementById('taskPriority').value),
            status: parseInt(document.getElementById('taskStatus').value)
        };
    }
    function addTask(task) {
        sendApiRequest('POST', 'https://localhost:44337/api/task', task)
            .then(data => {
                window.location.reload();
            })
            .catch(handleApiError);
    }
    function updateTask(taskId, task) {
        sendApiRequest('PUT', `https://localhost:44337/api/task/${taskId}`, task)
            .then(data => {
                window.location.reload();
            })
            .catch(handleApiError);
    }
    function sendApiRequest(method, url, data) {
        const token = localStorage.getItem('authToken');
        return fetch(url, {
            method: method,
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${token}`
            },
            body: JSON.stringify(data)
        }).then(response => {
            if (!response.ok) {
                throw new Error('Network response was not ok');
            }
            return response.json();
        });
    }
    function handleApiError(error) {
        console.error('API Error:', error);
    }
});

//=====================================
function logout() {
    localStorage.removeItem('authToken'); 
    window.location.href = 'login.html'; 
}
if (!localStorage.getItem('authToken')) {
    window.location.href = 'login.html'; 
}