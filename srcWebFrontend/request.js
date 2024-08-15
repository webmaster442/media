async function downloadContent(url) {
    if (typeof url !== 'string' || url.trim() === '') {
        throw new Error('Invalid URL: Must be a non-empty string');
    }
    try {
        const response = await fetch(url);
        if (!response.ok) {
            return `HTTP error! Status: ${response.status}`;
        }
        const text = await response.text();
        return text;
    }
    catch (error) {
        return `Error downloading content: ${error.message}`;
    }
}
