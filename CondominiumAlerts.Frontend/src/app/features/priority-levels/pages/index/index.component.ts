import { Component } from '@angular/core';
import { priorityDto } from '../../models/priorityDto';
import { NgFor, NgIf, Location } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { PriorityLevelService } from '../../../services/priorityLevel.service';
import { getPriorityLevelsQuery } from '../../models/getPriorityLevelsQuery';
import { CondominiumService } from '../../../condominiums/services/condominium.service';
import { getByIdPriorityLevelResponse } from '../../models/getByIdPriorityLevelResponse';
import { addPriorityLevelCommand } from '../../models/addPriorityLevelCommand';
import { ActivatedRoute } from '@angular/router';
import { CondominiumsLayoutComponent } from '../../../../shared/components/condominiums-layout/condominiums-layout.component';
import { Button } from 'primeng/button';
import { TableModule } from 'primeng/table';
@Component({
  selector: 'app-index',
  imports: [
    FormsModule,
    NgIf,
    CondominiumsLayoutComponent,
    Button,
    TableModule,
  ],
  templateUrl: './index.component.html',
  styleUrl: './index.component.css',
})
export class IndexComponent {
  constructor(
    private LevelService: PriorityLevelService,
    private condominiumService: CondominiumService,
    private route: ActivatedRoute,
    private location: Location
  ) {}

  condominiumId: string | null = null;
  page: getPriorityLevelsQuery = {
    pageNumber: 1,
    pageSize: 20,
    condominiumId: '',
  };

  priorities: priorityDto[] = [];
  showModal = false;
  editingPriority: getByIdPriorityLevelResponse | null = null;
  modalData: Partial<addPriorityLevelCommand> = {};

  ngOnInit() {
    this.loadPriorities();
  }
  loadPriorities(): void {
    this.condominiumId = this.route.snapshot.paramMap.get('condominiumId');
    this.page.condominiumId = this.condominiumId ?? '';

    this.LevelService.getPriorityLevels(this.page).subscribe(
      (result) => (this.priorities = result.data.priorities)
    );
  }
  openModal(priority: priorityDto | null = null) {
    this.showModal = true;
    if (priority?.condominiumId) {
      this.LevelService.getPriorityLevelById({
        id: priority.id,
        condominiumId: priority.condominiumId,
      }).subscribe({
        next: (result) => {
          this.editingPriority = result.data;
          this.modalData = {
            title: this.editingPriority.title!,
            priority: this.editingPriority.priority!,
            description: this.editingPriority.description!,
            condominiumId: this.editingPriority.condominiumId ?? '',
          };
           (this.editingPriority);
        },
        error: (err) => console.error('Error fetching priority level', err),
      });
    }
  }
  goBack() {
    this.location.back();
  }
  closeModal() {
    this.showModal = false;
    this.editingPriority = null;
    this.modalData = {};
  }

  savePriority() {
    if (this.editingPriority) {
      this.LevelService.updatePriorityLevel({
        id: this.editingPriority.id,
        title: this.modalData.title!,
        priority: this.modalData.priority!,
        description: this.modalData.description!,
        condominiumId: this.condominiumId ?? '',
      }).subscribe((result) => {
        this.priorities = this.priorities.map((p) =>
          p.id === this.editingPriority!.id
            ? ({ ...p, ...this.modalData } as priorityDto)
            : p
        );
        this.closeModal();
      });
    } else {
      const newPriority: addPriorityLevelCommand = {
        title: this.modalData.title!,
        priority: this.modalData.priority!,
        description: this.modalData.description!,
        condominiumId: this.condominiumId ?? '',
      };
      this.LevelService.postPriorityLevel(newPriority).subscribe((data) => {
        this.priorities.push({ ...newPriority, id: data.data.id });
        this.closeModal();
      });
    }
  }

  deletePriority(id: string) {
    this.LevelService.deletePriorityLevel({
      id: id,
      condominiumId: this.condominiumId ?? '',
    }).subscribe(() => {
      this.priorities = this.priorities.filter((p) => p.id !== id);
    });
  }
}
