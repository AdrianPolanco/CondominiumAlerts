export function AutoUnsubscribe() {
  return function (constructor: any) {
    const original = constructor.prototype.ngOnDestroy;

    constructor.prototype.ngOnDestroy = function () {
       ("AutoUnsubscribe");
      if (this.destroy$) {
        this.destroy$.next();
        this.destroy$.complete();
      }
      original && original.apply(this);
    };
  };
}
