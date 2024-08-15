async function downloadContent(url) {
    if (typeof url !== 'string' || url.trim() === '') {
        throw new Error('Invalid URL: Must be a non-empty string');
    }
    try {
        const response = await fetch(url);
        if (!response.ok) {
            return `Error: Http error ${response.status}`;
        }
        const text = await response.text();
        return text;
    }
    catch (error) {
        return `Error downloading content: ${error.message}`;
    }
}

async function apiCall(url) {
    var notify = document.getElementById('notify');
    let result = await downloadContent(url);
    notify.style.display = "block";
    if (result.startsWith('Error')) {
        notify.classList.add("w3-red");
    }
    else {
        notify.classList.add("w3-pale-green");
    }
    notify.innerHTML = result;
    setTimeout(function () {
        notify.style.display = "none";
    }, 2000);
}
