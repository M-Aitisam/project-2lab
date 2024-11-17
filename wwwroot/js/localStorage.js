function saveAnnouncements(announcements) {
    localStorage.setItem('announcements', JSON.stringify(announcements));
}

function loadAnnouncements() {
    return JSON.parse(localStorage.getItem('announcements') || '[]');
}
