import { DummyPipe } from './dummy.pipe';

describe('DummyPipe', () => {
  it('create an instance', () => {
    const pipe = new DummyPipe();
    expect(pipe).toBeTruthy();
  });
});
