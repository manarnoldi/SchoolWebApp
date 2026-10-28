export default <UiState>{
    darkMode: false,
    navbarVariant: 'navbar-light',
    sidebarSkin: 'sidebar-dark-primary',
    // Phones and tablets start with the menu closed.
    menuSidebarCollapsed: window.innerWidth < 992,
    controlSidebarCollapsed: true
    // screenSize: calculateWindowSize(window.innerWidth)
};

export interface UiState {
    darkMode: boolean;
    menuSidebarCollapsed: boolean;
    controlSidebarCollapsed: boolean;
    navbarVariant: string;
    sidebarSkin: string;
    screenSize: any;
}
