<template>
  <div class="homepage">
    <nav class="navbar">
      <div class="navbar-left">
        <h1>PixelCelebrate</h1>
      </div>
      <div class="navbar-right">
        <button @click="goToProfile" class="icon-button">
          <i class="fa fa-user"></i>
        </button>
        <button @click="logout" class="icon-button logout">
          <i class="fa fa-sign-out-alt"></i>
        </button>
      </div>
    </nav>
    <div class="homepage-content">
      <h1 v-if="currentUser" class="welcome-title">
        Welcome, {{ currentUser.firstName }}!
      </h1>
      <add-employee-modal
        :visible="showAddEmployeeModal"
        @submit="createUser"
        @close="showAddEmployeeModal = false"
      />
      <div v-if="currentUser?.isAdmin" class="notification-timing-container">
        <h2>Set notification timing</h2>
        <div class="input-container">
          <p>Birthday notification will be sent</p>
          <input
            type="number"
            v-model="notificationTiming"
            @input="onInputChange"
            min="1"
          />
          <p>days ahead.</p>
        </div>
        <button @click="setNotificationTiming">Set Number of days</button>
      </div>
      <div class="table-container">
        <div class="table-header">
          <h2>Employees</h2>
          <button
            v-if="currentUser?.isAdmin"
            @click="showAddEmployeeModal = true"
            class="add-employee-button"
          >
            Add Employee
          </button>
        </div>
        <table border="1" cellpadding="10">
          <thead>
            <tr>
              <th>Nr.</th>
              <th>First Name</th>
              <th>Last Name</th>
              <th v-if="currentUser?.isAdmin">Admin</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="(user, index) in users" :key="user.id">
              <td>{{ index + 1 }}</td>
              <td>{{ user.firstName }}</td>
              <td>{{ user.lastName }}</td>
              <td v-if="this.currentUser?.isAdmin">
                <input
                  type="checkbox"
                  :checked="user.isAdmin"
                  :disabled="user.id === currentUser.id"
                  @change="toggleAdmin(user.id, $event.target.checked)"
                />
              </td>
            </tr>
          </tbody>
        </table>

        <div class="pagination">
          <button @click="changePage('previous')" :disabled="currentPage === 1">
            &lt;
          </button>
          <span>Page {{ currentPage }} of {{ totalPages }}</span>
          <button
            @click="changePage('next')"
            :disabled="currentPage === totalPages"
          >
            &gt;
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import axios from "axios";
import AddEmployeeModal from "@/components/AddEmployeeModal.vue";

export default {
  name: "HomePage",
  components: {
    AddEmployeeModal,
  },
  data() {
    return {
      currentUser: null,
      users: [],
      currentPage: 1,
      totalPages: 1,
      usersPerPage: 15,
      showAddEmployeeModal: false,
      notificationTiming: null,
    };
  },
  methods: {
    async fetchUsers() {
      try {
        const response = await axios.get("http://localhost:5277/api/user", {
          params: { page: this.currentPage, pageSize: this.usersPerPage },
        });

        if (response.data && Array.isArray(response.data.users)) {
          this.users = response.data.users;
          this.totalPages = response.data.totalPages || 1;
        } else {
          console.error("Unexpected response format:", response.data);
          this.users = [];
          this.totalPages = 1;
        }
      } catch (error) {
        console.error("Error fetching users:", error);
      }
    },
    changePage(direction) {
      if (direction === "next" && this.currentPage < this.totalPages) {
        this.currentPage++;
      } else if (direction === "previous" && this.currentPage > 1) {
        this.currentPage--;
      }
      this.fetchUsers();
    },
    async fetchUserData() {
      try {
        const token = localStorage.getItem("token");

        if (!token) {
          throw new Error("No AuthToken found");
        }

        const response = await axios.get("http://localhost:5277/api/user/me", {
          headers: { Authorization: `Bearer ${token}` },
        });
        this.currentUser = response.data;
        localStorage.setItem("userData", JSON.stringify(this.currentUser));
      } catch (error) {
        console.error("Error fetching user details:", error);
        this.$router.push("/login");
      }
    },
    async fetchDaysBeforeBirthday(){
      try {
        const response = await axios.get("http://localhost:5277/api/birthday");

        this.notificationTiming = response.data.daysBeforeBirthday;     
      }catch (error) {
        console.error("Error fetching birthday details:", error);
      }
    },
    async setNotificationTiming(){
      try{
        await axios.patch('http://localhost:5277/api/birthday',{},{
          headers: {
            daysBeforeBirthday: this.notificationTiming
          }
        })
      }catch(error){
        console.error("Error updating daysBeforeBirthday status:", error);
      }
    },
    async toggleAdmin(userId, isAdmin) {
      try {
        await axios.patch(`http://localhost:5277/api/user/${userId}/isAdmin`, {
          isAdmin,
        });
        this.fetchUsers();
      } catch (error) {
        console.error("Error updating isAdmin status:", error);
      }
    },
    async createUser(newUser) {
      try {
        await axios.post("http://localhost:5277/api/user", newUser);
        this.fetchUsers();
        this.showAddEmployeeModal = false;
      } catch (error) {
        console.error("Error creating user:", error);
      }
    },
    goToProfile() {
      this.$router.push({ name: "ProfilePage" });
    },
    logout() {
      localStorage.removeItem("token");
      localStorage.removeItem("userData");
      this.$router.push("/login");
    },
  },
  async created() {
    await this.fetchUserData();
    await this.fetchUsers();
    await this.fetchDaysBeforeBirthday();
  },
};
</script>

<style scoped>
* {
  box-sizing: border-box;
  margin: 0;
  padding: 0;
}

html {
  height: 100%;
  overflow-x: hidden;
}
body {
  min-height: 100vh;
  overflow-x: hidden;
}

input[type="number"]::-webkit-inner-spin-button,
input[type="number"]::-webkit-outer-spin-button {
  -webkit-appearance: none;
  margin: 0;
}

input[type="number"] {
  -moz-appearance: textfield;
}

.navbar {
  display: flex;
  justify-content: space-between;
  align-items: center;
  background-color: #333;
  padding: 10px 20px;
  color: white;
  width: 100vw;
}

.navbar-left h1 {
  margin: 0;
}

.navbar-right {
  display: flex;
  flex-direction: row;
}

.icon-button {
  background: none;
  border: none;
  color: white;
  font-size: 18px;
  cursor: pointer;
}

.icon-button:hover {
  color: #ddd;
}

.icon-button.logout:hover {
  background-color: red;
}

.homepage-content {
  margin: 30px;
  padding: 30px;
  border: 1px solid black;
  border-radius: 10px;
  width: 500px;
}

.welcome-title {
  margin-top: 10px;
  margin-bottom: 40px;
}
.add-employee-button {
  padding: 5px 15px;
  margin: 10px;
  background-color: #6c757d;
  color: white;
  border: none;
  border-radius: 4px;
  font-size: 14px;
  cursor: pointer;
}

.add-employee-button:hover {
  background-color: #5a6268;
}

button {
  padding: 10px 20px;
  margin: 5px;
  background-color: #007bff;
  color: white;
  border: none;
  border-radius: 5px;
  cursor: pointer;
}

button:hover {
  background-color: #0056b3;
}

div {
  display: flex;
  justify-content: center;
  align-items: center;
  flex-direction: column;
}

table {
  border-collapse: collapse;
  width: 80%;
  margin-bottom: 20px;
}

th,
td {
  padding: 12px;
  text-align: left;
  border: 1px solid #ddd;
}

th {
  background-color: grey;
  color: white;
  font-weight: bold;
}

td {
  background-color: #f9f9f9;
}

tr:hover td {
  background-color: #f1f1f1;
}

.table-header{
  margin: 10px;
  flex-direction: row;
  justify-content: space-between;
}

.pagination {
  margin-top: 10px;
  flex-direction: row;
  width: 75%;
}

.pagination button {
  background-color: #007bff;
  color: white;
  border: none;
  cursor: pointer;
}

.pagination button:hover {
  background-color: #0056b3;
}

.pagination span {
  font-size: 12px;
  margin: 0 15px;
  width: 100%;
}

.pagination button:disabled {
  background-color: #cccccc;
  cursor: not-allowed;
}

input[type="checkbox"] {
  cursor: pointer;
}

.input-container {
  flex-direction: row;
}

.input-container p {
  margin: 5px;
}

.input-container input {
  width: 30px;
  text-align: center;
  padding: 5px;
}

.notification-timing-container h2 {
  margin: 0px;
}

.notification-timing-container button {
  margin-top: 10px;
  padding: 10px 20px;
  background-color: #28a745;
  color: white;
  border: none;
  border-radius: 5px;
  cursor: pointer;
}

.notification-timing-container button:hover {
  background-color: #218838;
}

.notification-timing-container p {
  margin-top: 10px;
  font-size: 14px;
}
</style>
