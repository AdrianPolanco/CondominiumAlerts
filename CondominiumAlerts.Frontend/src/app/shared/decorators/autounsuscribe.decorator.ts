export function AutoUnsubscribe() {
  return function (constructor: any) {
    const original = constructor.prototype.ngOnDestroy;
    console.log(`[AutoUnsubscribe] ${constructor.name} destruido`);

    // Guardar log en localStorage para verificar después de cambiar de página
    const logMessage = `[AutoUnsubscribe] ${constructor.name} destruido a las ${new Date().toLocaleTimeString()}`;
    console.log(logMessage);
    const logs = JSON.parse(sessionStorage.getItem('unsubscribeLogs') || '[]');
    logs.push(logMessage);
    sessionStorage.setItem('unsubscribeLogs', JSON.stringify(logs));

    constructor.prototype.ngOnDestroy = function () {
      console.log("AutoUnsubscribe");
      if (this.destroy$) {
        this.destroy$.next();
        this.destroy$.complete();
      }
      original && original.apply(this);
    };
  };
}
