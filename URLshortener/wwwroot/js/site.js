﻿function createShortUrl() {
    var originalUrl = document.getElementById("OriginalUrlCode").value;

    fetch('/api/ShortUrl/create', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({
            originalUrlCode: originalUrl 
        })
    })
        .then(response => {
            if (!response.ok) {
                throw new Error('HTTP error ' + response.status);
            }
            return response.json();
        })
        .then(data => {
            console.log('Successfully created short URL:', data);            
        })
        .catch(error => {
            console.error('Error creating short URL:', error);
        });
}
