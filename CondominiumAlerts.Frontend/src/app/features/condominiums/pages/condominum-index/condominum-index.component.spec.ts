import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CondominumIndexComponent } from './condominum-index.component';

describe('CondominumIndexComponent', () => {
  let component: CondominumIndexComponent;
  let fixture: ComponentFixture<CondominumIndexComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CondominumIndexComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CondominumIndexComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
