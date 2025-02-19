import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CondominiumsMainPageComponent } from './condominiums-main-page.component';

describe('CondominiumsMainPageComponent', () => {
  let component: CondominiumsMainPageComponent;
  let fixture: ComponentFixture<CondominiumsMainPageComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CondominiumsMainPageComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CondominiumsMainPageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
