import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ChatsDrawerComponent } from './chats-drawer.component';

describe('ChatsDrawerComponent', () => {
  let component: ChatsDrawerComponent;
  let fixture: ComponentFixture<ChatsDrawerComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ChatsDrawerComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ChatsDrawerComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
