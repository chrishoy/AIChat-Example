function ChatComponent() {
  return (
      <div className="grid grid-flow-col grid-rows-3 gap-4">
          <div className="row-span-3 border-blue-500 border-2">Spans 3 rows</div>
          <div className="col-span-2 border-red-500 border-2">Spans 2 cols</div>
          <div className="col-span-2 row-span-2 border-green-500 border-2">Spans 2 cols and 2 rows</div>
      </div>
  );
}

export default ChatComponent;