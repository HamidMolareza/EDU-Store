function previewImage(input) {
    const preview = document.getElementById('image-preview');
    const imageContainer = document.getElementById('image-container');
    if (input.files && input.files[0]) {
        const reader = new FileReader();

        reader.onload = function (e) {
            preview.src = e.target.result;
            preview.style.display = 'block';
            imageContainer.style.display = 'none'
            $("#image-validation-message").empty()
        }

        reader.readAsDataURL(input.files[0]);
    } else {
        preview.src = '#';
        preview.style.display = 'none';
        imageContainer.style.display = 'block'
    }
}

function getTimeZone() {
    return Intl.DateTimeFormat().resolvedOptions().timeZone; //For example: Asia/Tehran
}

function setCookie(name, value, days) {
    const expirationDate = days ? new Date(Date.now() + days * 24 * 60 * 60 * 1000) : null;
    const expires = expirationDate ? `; expires=${expirationDate.toUTCString()}` : '';
    document.cookie = `${name}=${value || ''}${expires}; path=/`;
}

function getCookie(name) {
    const nameEQ = name + '=';
    const cookies = document.cookie.split(';');
    for (let i = 0; i < cookies.length; i++) {
        let cookie = cookies[i].trim(); // Remove leading and trailing spaces
        if (cookie.startsWith(nameEQ)) {
            return cookie.substring(nameEQ.length);
        }
    }
    return null;
}

function setTimeZone() {
    const timeZoneKey = "TimeZone";
    if (!getCookie(timeZoneKey)) {
        setCookie(timeZoneKey, getTimeZone(), 1)
    }
}

setTimeZone();