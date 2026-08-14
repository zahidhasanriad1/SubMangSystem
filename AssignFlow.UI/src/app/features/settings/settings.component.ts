import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { finalize } from 'rxjs';
import { ButtonModule } from 'primeng/button';
import { DialogModule } from 'primeng/dialog';
import { InputTextModule } from 'primeng/inputtext';
import { TableModule } from 'primeng/table';
import { TextareaModule } from 'primeng/textarea';
import { TooltipModule } from 'primeng/tooltip';
import { ToastService } from '../../core/services/toast.service';
import { SettingService } from '../../data/services/setting.service';
import { SystemSetting } from '../../data/types/settings/system-setting';
import { UpsertSetting } from '../../data/types/settings/upsert-setting';

@Component({
  selector: 'app-settings',
  standalone: true,
  imports: [ReactiveFormsModule, ButtonModule, DialogModule, InputTextModule, TableModule, TextareaModule, TooltipModule],
  templateUrl: './settings.component.html',
  styleUrl: './settings.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class SettingsComponent {
  private readonly settingService = inject(SettingService);
  private readonly toast = inject(ToastService);
  private readonly formBuilder = inject(FormBuilder);

  readonly settings = signal<SystemSetting[]>([]);
  readonly loading = signal(true);
  readonly saving = signal(false);
  readonly dialogVisible = signal(false);
  readonly editingSetting = signal<SystemSetting | null>(null);
  readonly form = this.formBuilder.nonNullable.group({
    key: ['', [Validators.required, Validators.pattern(/^[A-Za-z0-9_.-]+$/)]],
    value: ['', Validators.required],
    description: ['']
  });

  constructor() {
    this.loadSettings();
  }

  openCreate(): void {
    this.editingSetting.set(null);
    this.form.controls.key.enable();
    this.form.reset({ key: '', value: '', description: '' });
    this.dialogVisible.set(true);
  }

  openEdit(setting: SystemSetting): void {
    this.editingSetting.set(setting);
    this.form.controls.key.disable();
    this.form.setValue({ key: setting.key, value: setting.value, description: setting.description ?? '' });
    this.dialogVisible.set(true);
  }

  save(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    const value = this.form.getRawValue();
    const model: UpsertSetting = { value: value.value, description: value.description || null };
    this.saving.set(true);
    this.settingService.upsert(value.key, model).pipe(finalize(() => this.saving.set(false))).subscribe(() => {
      this.toast.success('Application setting saved.');
      this.dialogVisible.set(false);
      this.loadSettings();
    });
  }

  private loadSettings(): void {
    this.loading.set(true);
    this.settingService.getSettings().pipe(finalize(() => this.loading.set(false))).subscribe((settings) => this.settings.set(settings));
  }
}
