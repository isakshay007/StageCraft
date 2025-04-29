document.addEventListener('DOMContentLoaded', function () {
    let currentProductionId = null;

    window.openCommentsModal = function (productionId) {
        currentProductionId = productionId;
        const modal = document.getElementById('commentModal');
        const modalContent = document.getElementById('commentModalContent');

        modal.style.display = 'block';
        modalContent.innerHTML = `
            <div class="text-center py-4">
                <div class="spinner-border text-primary" role="status">
                    <span class="visually-hidden">Loading...</span>
                </div>
            </div>
        `;

        fetch(`/Comments/GetCommentsPartial?productionId=${productionId}`)
            .then(response => {
                if (!response.ok) throw new Error('Failed to load comments');
                return response.text();
            })
            .then(html => {
                modalContent.innerHTML = html;
                attachEventHandlers();
            })
            .catch(error => {
                console.error('Error:', error);
                modalContent.innerHTML = `
                    <div class="alert alert-danger">
                        Error loading comments. 
                        <button onclick="openCommentsModal(${productionId})" class="btn btn-sm btn-outline-danger">Retry</button>
                    </div>
                `;
            });
    };

    function attachEventHandlers() {
        // Comment submission
        const commentForm = document.getElementById('commentForm');
        if (commentForm) {
            commentForm.addEventListener('submit', handleCommentSubmit);
        }

        // Comment deletion - now handling form submission instead of button click
        document.querySelectorAll('.delete-comment-form').forEach(form => {
            form.addEventListener('submit', handleCommentDelete);
        });
    }

    function handleCommentSubmit(e) {
        e.preventDefault();
        const form = e.target;
        const submitBtn = form.querySelector('button[type="submit"]');
        const originalText = submitBtn.innerHTML;

        submitBtn.disabled = true;
        submitBtn.innerHTML = `
            <span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span>
            Posting...
        `;

        fetch(form.action, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/x-www-form-urlencoded',
                'RequestVerificationToken': form.querySelector('input[name="__RequestVerificationToken"]').value
            },
            body: new URLSearchParams(new FormData(form))
        })
        .then(response => {
            if (!response.ok) throw new Error('Failed to post comment');
            return response.text();
        })
        .then(() => {
            openCommentsModal(currentProductionId);
            form.reset();
        })
        .catch(error => {
            console.error('Error:', error);
            alert('Failed to post comment. Please try again.');
        })
        .finally(() => {
            submitBtn.disabled = false;
            submitBtn.innerHTML = originalText;
        });
    }

    function handleCommentDelete(e) {
        e.preventDefault();
        if (!confirm('Are you sure you want to delete this comment?')) return;

        const form = e.target;
        const submitBtn = form.querySelector('button[type="submit"]');
        const originalText = submitBtn.innerHTML;

        submitBtn.disabled = true;
        submitBtn.innerHTML = `
            <span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span>
            Deleting...
        `;

        fetch(form.action, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/x-www-form-urlencoded',
                'RequestVerificationToken': form.querySelector('input[name="__RequestVerificationToken"]').value
            },
            body: new URLSearchParams(new FormData(form))
        })
        .then(response => {
            if (!response.ok) throw new Error('Failed to delete comment');
            return response.text();
        })
        .then(() => {
            openCommentsModal(currentProductionId);
        })
        .catch(error => {
            console.error('Error:', error);
            alert('Failed to delete comment. Please try again.');
        })
        .finally(() => {
            submitBtn.disabled = false;
            submitBtn.innerHTML = originalText;
        });
    }

    // Modal close handlers
    document.getElementById('closeModalBtn')?.addEventListener('click', () => {
        document.getElementById('commentModal').style.display = 'none';
    });

    window.addEventListener('click', (event) => {
        if (event.target === document.getElementById('commentModal')) {
            document.getElementById('commentModal').style.display = 'none';
        }
    });
});