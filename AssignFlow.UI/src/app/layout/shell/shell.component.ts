import { ChangeDetectionStrategy, Component, computed, inject, signal } from '@angular/core';
import { RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { AvatarModule } from 'primeng/avatar';
import { ButtonModule } from 'primeng/button';
import { TooltipModule } from 'primeng/tooltip';
import { AuthStore } from '../../core/auth/auth.store';
import { AppRole } from '../../data/enums/app-role';
import { NavItem } from './nav-item';

@Component({
  selector: 'app-shell',
  standalone: true,
  imports: [RouterOutlet, RouterLink, RouterLinkActive, AvatarModule, ButtonModule, TooltipModule],
  templateUrl: './shell.component.html',
  styleUrl: './shell.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class ShellComponent {
  readonly auth = inject(AuthStore);
  readonly mobileMenuOpen = signal(false);
  readonly currentYear = new Date().getFullYear();
  readonly navigation = computed(() => this.items.filter((item) => item.roles.includes(this.auth.role())));

  private readonly items: NavItem[] = [
    { label: 'Overview', icon: 'pi pi-th-large', route: '/dashboard', roles: Object.values(AppRole) },
    { label: 'People', icon: 'pi pi-users', route: '/users', roles: [AppRole.Admin] },
    { label: 'Academics', icon: 'pi pi-building-columns', route: '/academic', roles: Object.values(AppRole) },
    { label: 'Assignments', icon: 'pi pi-file-edit', route: '/assignments', roles: Object.values(AppRole) },
    { label: 'Submissions', icon: 'pi pi-inbox', route: '/submissions', roles: Object.values(AppRole) },
    { label: 'Settings', icon: 'pi pi-cog', route: '/settings', roles: [AppRole.Admin] }
  ];

  openMobileMenu(): void {
    this.mobileMenuOpen.set(true);
  }

  closeMobileMenu(): void {
    this.mobileMenuOpen.set(false);
  }
}
