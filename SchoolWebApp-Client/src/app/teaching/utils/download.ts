// Saving a downloaded file, and reading the API's message out of a failed
// blob request (the error body arrives as a Blob, not JSON).

export function saveBlob(blob: Blob, fileName: string): void {
    let url = URL.createObjectURL(blob);
    let a = document.createElement('a');
    a.href = url;
    a.download = fileName;
    document.body.appendChild(a);
    a.click();
    a.remove();
    setTimeout(() => URL.revokeObjectURL(url), 1000);
}

export async function blobError(err: any, fallback: string): Promise<string> {
    try {
        if (err?.error instanceof Blob) {
            let body = JSON.parse(await err.error.text());
            return body?.message || fallback;
        }
        return err?.error?.message || fallback;
    } catch {
        return fallback;
    }
}

// Characters Windows will not have in a file name.
export function safeFileName(name: string): string {
    return name.replace(/[\\/:*?"<>|]+/g, ' ').replace(/\s+/g, ' ').trim();
}
