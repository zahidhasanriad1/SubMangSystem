import { DatePipe } from '@angular/common';
import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { debounceTime, distinctUntilChanged, finalize } from 'rxjs';
import { ButtonModule } from 'primeng/button';
import { DialogModule } from 'primeng/dialog';
import { InputTextModule } from 'primeng/inputtext';
import { PasswordModule } from 'primeng/password';
import { SelectModule } from 'primeng/select';
import { TableModule } from 'primeng/table';
import { TagModule } from 'primeng/tag';
import { ToggleSwitchModule } from 'primeng/toggleswitch';
import { TooltipModule } from 'primeng/tooltip';
import { ToastService } from '../../core/services/toast.service';
import { AppRole } from '../../data/enums/app-role';
import { AdminService } from '../../data/services/admin.service';
import { CreateUser } from '../../data/types/users/create-user';
import { UpdateUser } from '../../data/types/users/update-user';
import { User } from '../../data/types/users/user';

@Component({
  selector: 'app-users',
  standalone: true,
  imports: [
    DatePipe,
    ReactiveFormsModule,
    ButtonModule,
    DialogModule,
    InputTextModule,
    PasswordModule,
    SelectModule,
    TableModule,
    TagModule,
    ToggleSwitchModule,
    TooltipModule
  ],
  templateUrl: './users.component.html',
  styleUrl: './users.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class UsersComponent {
  private readonly adminService = inject(AdminService);
  private readonly toast = inject(ToastService);
  private readonly formBuilder = inject(FormBuilder);

  readonly users = signal<User[]>([]);
  readonly totalRecords = signal(0);
  readonly loading = signal(false);
  readonly saving = signal(false);
  readonly dialogVisible = signal(false);
  readonly editingUser = signal<User | null>(null);
  readonly roles = Object.values(AppRole);
  readonly search = this.formBuilder.nonNullable.control('');
  readonly form = this.formBuilder.nonNullable.group({
    fullName: ['', [Validators.required, Validators.maxLength(120)]],
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required, Validators.minLength(8)]],
    role: [AppRole.Student as string, Validators.required],
    isActive: [true]
  });

  private page = 1;
  private pageSize = 10;

  constructor() {
    this.search.valueChanges.pipe(debounceTime(300), distinctUntilChanged(), takeUntilDestroyed()).subscribe(() => {
      this.page = 1;
      this.loadUsers();
    });
  }

  load(event?: { first?: number | null; rows?: number | null }): void {
    if (event) {
      this.pageSize = event.rows ?? this.pageSize;
      this.page = Math.floor((event.first ?? 0) / this.pageSize) + 1;
    }
    this.loadUsers();
  }

  openCreate(): void {
    this.editingUser.set(null);
    this.form.controls.email.enable();
    this.form.controls.password.enable();
    this.form.controls.email.setValidators([Validators.required, Validators.email]);
    this.form.controls.password.setValidators([Validators.required, Validators.minLength(8)]);
    this.form.reset({ fullName: '', email: '', password: '', role: AppRole.Student, isActive: true });
    this.dialogVisible.set(true);
  }

  openEdit(user: User): void {
    this.editingUser.set(user);
    this.form.controls.email.clearValidators();
    this.form.controls.password.clearValidators();
    this.form.controls.email.disable();
    this.form.controls.password.disable();
    this.form.patchValue({ fullName: user.fullName, email: user.email, password: '', role: user.role, isActive: user.isActive });
    this.form.updateValueAndValidity();
    this.dialogVisible.set(true);
  }

  save(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const value = this.form.getRawValue();
    const user = this.editingUser();
    this.saving.set(true);
    const request = user
      ? this.adminService.updateUser(user.userId, { fullName: value.fullName, role: value.role, isActive: value.isActive } satisfies UpdateUser)
      : this.adminService.createUser({ fullName: value.fullName, email: value.email, password: value.password, role: value.role } satisfies CreateUser);

    request.pipe(finalize(() => this.saving.set(false))).subscribe(() => {
      this.toast.success(user ? 'User profile updated.' : 'User account created.');
      this.dialogVisible.set(false);
      this.loadUsers();
    });
  }

  initials(name: string): string {
    return name.split(' ').filter(Boolean).slice(0, 2).map((part) => part[0]).join('').toUpperCase();
  }

  private loadUsers(): void {
    this.loading.set(true);
    this.adminService.getUsers(this.page, this.pageSize, this.search.value.trim())
      .pipe(finalize(() => this.loading.set(false)))
      .subscribe((result) => {
        this.users.set(result.items);
        this.totalRecords.set(result.totalCount);
      });
  }
}
