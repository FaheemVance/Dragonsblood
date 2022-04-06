using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using DragonsBlood.Data;
using DragonsBlood.Data.Extensions;
using DragonsBlood.Models.ChatModels;

namespace DragonsBlood.Controllers.Resources
{
    public class ChatController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: ApplicationRoles
        public ActionResult Index()
        {
            return View(CreateIndexModel());
        }

        private ChatIndexModel CreateIndexModel()
        {
            using (var context = new ApplicationDbContext())
            {
                var model = new ChatIndexModel();
                model.Roles = context.ChatRoles.ToList();
                model.RoomPermissions = context.RoomPermissions.ToList();
                model.Rooms = context.ChatRooms.ToList();
                model.UserRoles = context.UserChatRoles.ToList();

                return model;
            }
        }

        [HttpGet]
        public ActionResult CreateRoomPermission()
        {
            using (var context = new ApplicationDbContext())
            {
                var model = new ChatRoomPermissionModel();
                var permissions = context.ChatRoles.Select(s => s.Name).ToList();
                var rooms = context.ChatRooms.Select(s => s.Name).ToList();

                var roomPermissions = context.RoomPermissions.ToList();

                foreach (var permission in roomPermissions)
                {
                    if (rooms.Contains(permission.Room.Name))
                        rooms.Remove(permission.Room.Name);
                }

                model.Permissions = permissions;
                model.Rooms = rooms;

                return View(model);
            }
        }

        [HttpPost]
        public ActionResult CreateRoomPermission([Bind(Include = "Rooms, Permissions, SelectedRoom, SelectedPermission")]ChatRoomPermissionModel model)
        {
            using (var context = new ApplicationDbContext())
            {
                var room = context.ChatRooms.FirstOrDefault(r => r.Name == model.SelectedRoom);
                var group = context.ChatRoles.FirstOrDefault(p => p.Name == model.SelectedPermission);

                if (room == null || group == null)
                    return RedirectToAction("Index");

                var permission = new ChatRoomPermission();
                permission.Role = group;
                permission.Room = room;

                context.RoomPermissions.Add(permission);
                context.SaveChanges();

                return RedirectToAction("Index");
            }
        }

        // GET: ApplicationRoles/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ApplicationRoles/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Name,Description")] ChatRole applicationRole)
        {
            if (ModelState.IsValid)
            {
                db.ChatRoles.Add(applicationRole);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(applicationRole);
        }

        // GET: ApplicationRoles/Edit/5
        public ActionResult Edit(int id)
        {
            ChatRole applicationRole = db.ChatRoles.Find(id);
            if (applicationRole == null)
            {
                return HttpNotFound();
            }
            return View(applicationRole);
        }

        // POST: ApplicationRoles/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Name,Description")] ChatRole applicationRole)
        {
            if (ModelState.IsValid)
            {
                var entry = db.ChatRoles.First(r => r.Name == applicationRole.Name);
                db.Entry(entry).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(applicationRole);
        }


        public ActionResult DeleteRoom(int id)
        {
            if(!User.IsAdmin())
                return RedirectToAction("Index", "Home");

            using (var context = new ApplicationDbContext())
            {
                var room = context.ChatRooms.FirstOrDefault(c => c.Id == id);

                if (room == null)
                    return RedirectToAction("Index");

                var roomPermission = context.RoomPermissions.FirstOrDefault(r => r.Room.Id == room.Id);

                context.MessageArchive.AddRange(
                    room.Messages.Select(
                        s =>
                            new ChatMessageArchive()
                            {
                                Message = s.Message,
                                RoomName = room.Name,
                                TimeStamp = s.TimeStamp,
                                User = s.User
                            }));

                context.ChatMessages.RemoveRange(room.Messages);
                context.RoomUsers.RemoveRange(room.RoomUsers);
                if(roomPermission != null)
                    context.RoomPermissions.Remove(roomPermission);
                context.ChatRooms.Remove(room);
                context.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        public ActionResult DeleteUserGroup(int id)
        {
            using (var context = new ApplicationDbContext())
            {
                if (!User.IsAdmin())
                    return RedirectToAction("Index", "Home");

                var group = context.ChatRoles.FirstOrDefault(r => r.Id == id);

                if (group == null)
                    return RedirectToAction("Index");

                context.ChatRoles.Remove(group);
                context.SaveChanges();

                return RedirectToAction("Index");
            }
        }

        public ActionResult DeleteRoomPermission(int id)
        {
            using (var context = new ApplicationDbContext())
            {
                if (!User.IsAdmin())
                    return RedirectToAction("Index", "Home");

                var group = context.RoomPermissions.FirstOrDefault(r => r.Id == id);

                if (group == null)
                    return RedirectToAction("Index");

                context.RoomPermissions.Remove(group);
                context.SaveChanges();

                return RedirectToAction("Index");
            }
        }

        [HttpGet]
        public ActionResult EditRoom(int id)
        {
            using (var context = new ApplicationDbContext())
            {
                var room = context.ChatRooms.FirstOrDefault(r => r.Id == id);

                if (room == null)
                    return RedirectToAction("Index");

                return View(room);
            }
        }

        [HttpGet]
        public ActionResult EditUserGroup(int id)
        {
            using (var context = new ApplicationDbContext())
            {
                var group = context.ChatRoles.FirstOrDefault(r => r.Id == id);

                if (group == null)
                    return RedirectToAction("Index");

                return View(group);
            }
        }

        [HttpPost]
        public ActionResult EditRoom(ChatRoom room)
        {
            using (var context = new ApplicationDbContext())
            {
                var existingRoom = context.ChatRooms.FirstOrDefault(r => r.Id == room.Id);

                if (existingRoom == null)
                    return RedirectToAction("Index");

                existingRoom.Name = room.Name;
                context.SaveChanges();

                return View(room);
            }
        }

        [HttpPost]
        public ActionResult EditUserGroup(ChatRole role)
        {
            using (var context = new ApplicationDbContext())
            {
                var group = context.ChatRoles.FirstOrDefault(r => r.Id == role.Id);

                if (group == null)
                    return RedirectToAction("Index");

                group.Name = role.Name;
                group.Description = role.Description;

                context.SaveChanges();

                return View(group);
            }
        }



        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
