

function toggleSidebar() {
    const sidebar = document.getElementById('sidebar');
    const overlay = document.getElementById('sidebarOverlay');
    sidebar.classList.toggle('active');
    overlay.classList.toggle('active');
}

document.addEventListener('DOMContentLoaded', () => {
    // Set active menu item based on current page
    const currentPath = window.location.pathname.toLowerCase();
    const menuLinks = document.querySelectorAll('.sidebar-menu a');
    menuLinks.forEach(link => {
        const href = link.getAttribute('href')?.toLowerCase();
        if (href && currentPath === href) {
            link.classList.add('active');
        } else {
            link.classList.remove('active');
        }
    });
});