import { AgeFromDateOfBirthPipe } from './age-from-date-of-birth.pipe';

describe('AgeFromDateOfBirthPipe', () => {
  it('create an instance', () => {
    const pipe = new AgeFromDateOfBirthPipe();
    expect(pipe).toBeTruthy();
  });
});
