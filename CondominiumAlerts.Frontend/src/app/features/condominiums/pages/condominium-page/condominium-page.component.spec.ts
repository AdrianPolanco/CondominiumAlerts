import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CondominiumPageComponent } from './condominium-page.component';

describe('CondominiumPageComponent', () => {
  let component: CondominiumPageComponent;
  let fixture: ComponentFixture<CondominiumPageComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CondominiumPageComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CondominiumPageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
