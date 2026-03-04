class JsFunctions {
    public static isMobile(): boolean {
        return window.innerWidth <= 1024;
    }

    public static getWindowWidth(): number {
        return window.innerWidth;
    }

    public static detectTheme(): string | null {
        return localStorage.getItem('theme');
    }

    public static registerResize(dotNetReference: any): void {
        window.addEventListener("resize", () => {
            dotNetReference.invokeMethodAsync("OnWindowResize", window.innerWidth);
        });
    }

    public static downloadFileFromBytes(filename: string, bytes: number[]): void {
        const uint8Array = new Uint8Array(bytes);
        const blob = new Blob([uint8Array], { type: "application/octet-stream" });

        const url = URL.createObjectURL(blob);
        const a = document.createElement("a");

        a.href = url;
        a.download = filename;
        document.body.appendChild(a);
        a.click();
        a.remove();

        URL.revokeObjectURL(url);
    }

    public static getOuterHtml(selector: string): string {
        const el = document.querySelector(selector) as HTMLElement | null;
        if (!el) return "";

        const cloned = el.cloneNode(true) as HTMLElement;

        const headers = cloned.querySelectorAll("th");
        if (headers.length > 0) {
            headers[headers.length - 1].remove();
        }

        const rows = cloned.querySelectorAll("tr");
        rows.forEach(row => {
            const cells = row.querySelectorAll("td");
            if (cells.length > 0) {
                cells[cells.length - 1].remove();
            }
        });

        return `<html>
            <head>
                <title>Report</title>
                <style>
                    table { border-collapse: collapse; width: 100%; font-family: Arial, sans-serif; }
                    th, td { border: 1px solid #ddd; padding: 8px; text-align: center; }
                    th { background: #f6f6f6; font-weight: 700; }
                </style>
            </head>
            <body>
                ${cloned.outerHTML}
            </body>
            </html>`;
    }


    public static openPrintWindow(html: string): void {
        const w = window.open("", "_blank");

        if (w) {
            w.document.write(html);
            w.document.close();
            w.focus();

            setTimeout(() => {
                w.print();
            }, 300);
        }
    }
}

(window as any).JsFunctions = JsFunctions;