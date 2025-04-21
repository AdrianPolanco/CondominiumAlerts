import { ComponentFixture, TestBed } from '@angular/core/testing';

import { JoinwithTokenComponent } from './joinwith-token.component';

describe('JoinwithTokenComponent', () => {
  let component: JoinwithTokenComponent;
  let fixture: ComponentFixture<JoinwithTokenComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [JoinwithTokenComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(JoinwithTokenComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
