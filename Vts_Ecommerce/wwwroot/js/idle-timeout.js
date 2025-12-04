// Idle timeout detection and session keepalive
(function () {
    const IDLE_TIMEOUT_MS = 2 * 60 * 1000; // 2 minutes
    let idleTimer = null;
    let isIdle = false;

    // Reset idle timer on user activity
    function resetIdleTimer() {
        if (idleTimer) clearTimeout(idleTimer);
        isIdle = false;

        idleTimer = setTimeout(() => {
            isIdle = true;
            showIdleModal();
        }, IDLE_TIMEOUT_MS);
    }

    // Show idle modal with options
    function showIdleModal() {
        const modalHtml = `
            <div id="idleModal" class="modal fade" role="dialog">
                <div class="modal-dialog" role="document">
                    <div class="modal-content">
                        <div class="modal-header">
                            <h5 class="modal-title">Session Timeout</h5>
                        </div>
                        <div class="modal-body">
                            <p>Your session will expire due to inactivity. Click "Continue" to stay logged in or "Logout" to exit.</p>
                        </div>
                        <div class="modal-footer">
                            <button type="button" class="btn btn-primary" id="continueBtn">Continue</button>
                            <button type="button" class="btn btn-secondary" id="logoutBtn">Logout</button>
                        </div>
                    </div>
                </div>
            </div>
        `;

        // Remove existing modal if any
        const existing = document.getElementById('idleModal');
        if (existing) existing.remove();

        document.body.insertAdjacentHTML('beforeend', modalHtml);
        const modal = new bootstrap.Modal(document.getElementById('idleModal'), { backdrop: 'static', keyboard: false });
        modal.show();

        document.getElementById('continueBtn').addEventListener('click', () => {
            sendKeepalive();
            modal.hide();
            resetIdleTimer();
        });

        document.getElementById('logoutBtn').addEventListener('click', () => {
            window.location.href = '/Account/Logout';
        });
    }

    // Send AJAX keepalive ping
    function sendKeepalive() {
        fetch('/Account/Keepalive', {
            method: 'POST',
            headers: { 'X-Requested-With': 'XMLHttpRequest' }
        }).catch(() => {
            console.warn('Keepalive ping failed');
        });
    }

    // Bind activity events
    const events = ['mousedown', 'keydown', 'scroll', 'touchstart', 'click'];
    events.forEach(event => {
        document.addEventListener(event, resetIdleTimer, true);
    });

    // Initialize
    resetIdleTimer();
})();
