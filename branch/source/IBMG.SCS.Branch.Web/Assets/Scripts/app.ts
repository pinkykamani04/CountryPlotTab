class JsFunctions {
    public static isMobile(): boolean {
        return window.innerWidth <= 1024;
    }

    public static getWindowWidth(): number {
        return window.innerWidth;
    }

    public static detectTheme(): string {
        return localStorage.getItem('theme');
    }

    public static registerResize(dotNetReference: any): void {
        window.addEventListener("resize", () => {
            dotNetReference.invokeMethodAsync("OnWindowResize", window.innerWidth);
        });
    }
}

(window as any).JsFunctions = JsFunctions;