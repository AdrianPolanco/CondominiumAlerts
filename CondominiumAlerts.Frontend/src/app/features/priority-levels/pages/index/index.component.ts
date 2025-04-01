import { Component } from '@angular/core';
import { priorityDto } from '../../models/priorityDto';
import { NgFor, NgIf } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { priorityLevelService } from '../../../services/services.service';
@Component({
  selector: 'app-index',
  imports: [NgFor, FormsModule,NgIf],
  templateUrl: './index.component.html',
  styleUrl: './index.component.css'
})
export class IndexComponent {

constructor(private LevelService: priorityLevelService){

}

  priorities: priorityDto[] = [
    { id: '1', title: 'High Priority', priority: 1, condominiumId: ""},
    { id: '2', title: 'Medium Priority', priority: 2, condominiumId: ""},
    { id: '3', title: 'Low Priority', priority: 3, condominiumId: ""},
  ];

  showModal = false;
  editingPriority: priorityDto | null = null; 
  modalData : Partial<priorityDto> = {};

  openModal(priority: priorityDto | null = null) {
    this.showModal = true;
 if(priority != null && priority.condominiumId){
  this.LevelService.getPriorityLevelById({
      id: priority?.id,
      condominiumId: priority.condominiumId
    }).subscribe((data) => data)
    this.editingPriority = priority;
 }
    
    this.modalData = priority ? { ...priority } : { title: '', priority: 1, id: '', condominiumId: '' };
  }

  closeModal() {
    this.showModal = false;
    this.editingPriority = null;
    this.modalData = {};
  }
  savePriority() {
    if (this.editingPriority) {
      this.priorities = this.priorities.map(p =>
        p.id === this.editingPriority!.id ? { ...p, ...this.modalData } as priorityDto : p
      );
    } else {
      const newPriority: priorityDto = {
        id: Math.random().toString(),
        title: this.modalData.title!,
        priority: this.modalData.priority!,
        condominiumId: "gojnnjgdfjbngfdjbn",
      };
      this.priorities.push(newPriority);
    }
    this.closeModal();
  }

  deletePriority(id: string) {
    this.priorities = this.priorities.filter(p => p.id !== id);
  }
}
